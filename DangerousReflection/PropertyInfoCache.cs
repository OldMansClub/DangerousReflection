using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DangerousReflection {
	/// <summary>
	/// Cache for PropertyInfo
	/// </summary>
	internal class PropertyInfoCache {
		public Func<object, object> Getter => _getter;
		public Action<object, object> Setter => _setter;
		private Func<object, object> _getter;
		private Action<object, object> _setter;

		/// <summary>
		/// Make prperty getter by lambda compile
		/// </summary>
		public static Func<object, object> LambdaMakeGetter(PropertyInfo propertyInfo) {
			if (!propertyInfo.CanRead) {
				return null;
			}
			var instance = Expression.Parameter(typeof(object), "instance");
			var getMethod = propertyInfo.GetGetMethod(true);
			var instanceCast = getMethod.IsStatic ? null :
				Expression.Convert(instance, propertyInfo.DeclaringType);
			var propertyAccess = Expression.Property(instanceCast, propertyInfo);
			var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));
			var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instance);
			return lambda.Compile();
		}

		/// <summary>
		/// Make prperty setter by lambda compile
		/// </summary>
		public static Action<object, object> LambdaMakeSetter(PropertyInfo propertyInfo) {
			if (!propertyInfo.CanWrite) {
				return null;
			}
			var instance = Expression.Parameter(typeof(object), "instance");
			var value = Expression.Parameter(typeof(object), "value");
			var setMethod = propertyInfo.GetSetMethod(true);
			var instanceCast = setMethod.IsStatic ? null :
				Expression.Convert(instance, propertyInfo.DeclaringType);
			var propertyAccess = Expression.Property(instanceCast, propertyInfo);
			var castValue = Expression.Convert(value, propertyInfo.PropertyType);
			var assign = Expression.Assign(propertyAccess, castValue);
			var lambda = Expression.Lambda<Action<object, object>>(assign, instance, value);
			return lambda.Compile();
		}

		/// <summary>
		/// Make property getter by emit
		/// </summary>
		public static Func<object, object> EmitMakeGetter(PropertyInfo propertyInfo) {
			if (!propertyInfo.CanRead) {
				return null;
			}
			var declaringType = propertyInfo.DeclaringType;
			var declaringTypeIsValueType = declaringType.IsValueType;
			var propertyTypeIsValueType = propertyInfo.PropertyType.IsValueType;
			var methodName = $"__dynamicGet_{propertyInfo.Name}_From_{declaringType.Name}";
			var method = new DynamicMethod(methodName,
				typeof(object), new[] { typeof(object) }, true);
			var gen = method.GetILGenerator();
			var getMethod = propertyInfo.GetMethod;
			if (getMethod.IsStatic) {
				gen.Emit(OpCodes.Call, getMethod);
			} else {
				gen.Emit(OpCodes.Ldarg_0);
				if (declaringTypeIsValueType) {
					gen.Emit(OpCodes.Unbox, declaringType);
					gen.Emit(OpCodes.Call, getMethod);
				} else {
					gen.Emit(OpCodes.Castclass, declaringType);
					gen.Emit(OpCodes.Callvirt, getMethod);
				}
			}
			if (propertyTypeIsValueType) {
				gen.Emit(OpCodes.Box, propertyInfo.PropertyType);
			}
			gen.Emit(OpCodes.Ret);
			return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
		}

		/// <summary>
		/// Make property setter by emit
		/// </summary>
		public static Action<object, object> EmitMakeSetter(PropertyInfo propertyInfo) {
			if (!propertyInfo.CanWrite) {
				return null;
			}
			var declaringType = propertyInfo.DeclaringType;
			var declaringTypeIsValueType = declaringType.IsValueType;
			var propertyTypeIsValueType = propertyInfo.PropertyType.IsValueType;
			var methodName = $"__dynamicSet_{propertyInfo.Name}_To_{declaringType.Name}";
			var method = new DynamicMethod(methodName,
				typeof(void), new[] { typeof(object), typeof(object) }, true);
			var gen = method.GetILGenerator();
			var setMethod = propertyInfo.SetMethod;
			if (!setMethod.IsStatic) {
				gen.Emit(OpCodes.Ldarg_0);
				if (declaringTypeIsValueType) {
					gen.Emit(OpCodes.Unbox, declaringType);
				} else {
					gen.Emit(OpCodes.Castclass, declaringType);
				}
			}
			gen.Emit(OpCodes.Ldarg_1);
			if (propertyTypeIsValueType) {
				gen.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
			} else {
				gen.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
			}
			if (setMethod.IsStatic || declaringTypeIsValueType) {
				gen.Emit(OpCodes.Call, setMethod);
			} else {
				gen.Emit(OpCodes.Callvirt, setMethod);
			}
			gen.Emit(OpCodes.Ret);
			return (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
		}

		/// <summary>
		/// Initialize
		/// </summary>
		public PropertyInfoCache(PropertyInfo propertyInfo) {
			_getter = EmitMakeGetter(propertyInfo);
			_setter = EmitMakeSetter(propertyInfo);
		}
	}
}
