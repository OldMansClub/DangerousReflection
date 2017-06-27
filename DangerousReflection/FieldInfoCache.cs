using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DangerousReflection {
	/// <summary>
	/// Cache for FieldInfo
	/// </summary>
	internal class FieldInfoCache {
		public Func<object, object> Getter => _getter;
		public Action<object, object> Setter => _setter;
		private Func<object, object> _getter;
		private Action<object, object> _setter;

		/// <summary>
		/// Make field getter by lambda compile
		/// </summary>
		public static Func<object, object> LambdaMakeGetter(FieldInfo fieldInfo) {
			var instance = Expression.Parameter(typeof(object), "instance");
			var instanceCast = fieldInfo.IsStatic ? null :
				Expression.Convert(instance, fieldInfo.DeclaringType);
			var fieldAccess = Expression.Field(instanceCast, fieldInfo);
			var castFieldValue = Expression.Convert(fieldAccess, typeof(object));
			var lambda = Expression.Lambda<Func<object, object>>(castFieldValue, instance);
			return lambda.Compile();
		}

		/// <summary>
		/// Make field setter by lambda compile
		/// </summary>
		public static Action<object, object> LambdaMakeSetter(FieldInfo fieldInfo) {
			var instance = Expression.Parameter(typeof(object), "instance");
			var value = Expression.Parameter(typeof(object), "value");
			var instanceCast = fieldInfo.IsStatic ? null :
				Expression.Convert(instance, fieldInfo.DeclaringType);
			var fieldAccess = Expression.Field(instanceCast, fieldInfo);
			var castValue = Expression.Convert(value, fieldInfo.FieldType);
			var assign = Expression.Assign(fieldAccess, castValue);
			var lambda = Expression.Lambda<Action<object, object>>(assign, instance, value);
			return lambda.Compile();
		}

		/// <summary>
		/// Make field getter by emit
		/// </summary>
		public static Func<object, object> EmitMakeGetter(FieldInfo fieldInfo) {
			var declaringType = fieldInfo.DeclaringType;
			var methodName = $"__dynamicGet_{fieldInfo.Name}_From_{declaringType.Name}";
			var method = new DynamicMethod(methodName,
				typeof(object), new[] { typeof(object) }, true);
			var gen = method.GetILGenerator();
			if (fieldInfo.IsStatic) {
				gen.Emit(OpCodes.Ldsfld, fieldInfo);
			} else {
				gen.Emit(OpCodes.Ldarg_0);
				if (declaringType.GetTypeInfo().IsValueType) {
					gen.Emit(OpCodes.Unbox, declaringType);
				} else {
					gen.Emit(OpCodes.Castclass, declaringType);
				}
				gen.Emit(OpCodes.Ldfld, fieldInfo);
			}
			if (fieldInfo.FieldType.GetTypeInfo().IsValueType) {
				gen.Emit(OpCodes.Box, fieldInfo.FieldType);
			}
			gen.Emit(OpCodes.Ret);
			return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
		}

		/// <summary>
		/// Make field setter by emit
		/// </summary>
		public static Action<object, object> EmitMakeSetter(FieldInfo fieldInfo) {
			var declaringType = fieldInfo.DeclaringType;
			var methodName = $"__dynamicSet_{fieldInfo.Name}_To_{declaringType.Name}";
			var method = new DynamicMethod(methodName,
				typeof(void), new[] { typeof(object), typeof(object) }, true);
			var gen = method.GetILGenerator();
			if (!fieldInfo.IsStatic) {
				gen.Emit(OpCodes.Ldarg_0);
				if (declaringType.GetTypeInfo().IsValueType) {
					gen.Emit(OpCodes.Unbox, declaringType);
				} else {
					gen.Emit(OpCodes.Castclass, declaringType);
				}
			}
			gen.Emit(OpCodes.Ldarg_1);
			if (fieldInfo.FieldType.GetTypeInfo().IsValueType) {
				gen.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
			} else {
				gen.Emit(OpCodes.Castclass, fieldInfo.FieldType);
			}
			if (fieldInfo.IsStatic) {
				gen.Emit(OpCodes.Stsfld, fieldInfo);
			} else {
				gen.Emit(OpCodes.Stfld, fieldInfo);
			}
			gen.Emit(OpCodes.Ret);
			return (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
		}

		/// <summary>
		/// Initialize
		/// </summary>
		public FieldInfoCache(FieldInfo fieldInfo) {
			_getter = EmitMakeGetter(fieldInfo);
			_setter = EmitMakeSetter(fieldInfo);
		}
	}
}
