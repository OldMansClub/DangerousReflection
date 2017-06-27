using DangerousReflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Reflection {
	/// <summary>
	/// Quick reflection methods
	/// </summary>
	public static class DangerousReflectionExtensions {
		private static TypeInfoCache[] _typeInfoCacheArray = new TypeInfoCache[1];
		private static int _typeInfoCacheIndex = 0; // start from 1
		private static FieldInfoCache[] _fieldInfoCacheArray = new FieldInfoCache[1];
		private static int _fieldInfoCacheIndex = 0; // start from 1
		private static PropertyInfoCache[] _propertyInfoCacheArray = new PropertyInfoCache[1];
		private static int _propertyInfoCacheIndex = 0; // start from 1
		private static MethodInfoCache[] _methodInfoCacheArray = new MethodInfoCache[1];
		private static int _methodInfoCacheIndex = 0; // start from 1

		/// <summary>
		/// Get cached information for Type
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static TypeInfoCache GetTypeInfoCache(Type type) {
			// null check
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			// for thread safe
			var cacheArray = _typeInfoCacheArray;
			// get cache by index
			var cacheIndex = ObjectHeaderAccessor.GetIndex(type);
			if (cacheIndex > 0 && cacheIndex < cacheArray.Length) {
				var cached = cacheArray[cacheIndex];
				if (cached != null) {
					// cache array may replaced by other thread,
					// it so we need create the cache entry again
					return cached;
				}
			}
			// get a new cache index
			if (cacheIndex == 0) {
				if (_typeInfoCacheIndex > ObjectHeaderAccessor.MaxIndex) {
					return null;
				}
				cacheIndex = Interlocked.Increment(ref _typeInfoCacheIndex);
				if (cacheIndex > ObjectHeaderAccessor.MaxIndex) {
					return null;
				}
			}
			// create new cache array if size not enough
			if (cacheIndex >= cacheArray.Length) {
				var newCacheArray = new TypeInfoCache[
					Math.Min(
						Math.Max(cacheArray.Length * 2, cacheIndex + 1),
						ObjectHeaderAccessor.MaxIndex + 1)];
				Array.Copy(cacheArray, newCacheArray, cacheArray.Length);
				cacheArray = newCacheArray;
				_typeInfoCacheArray = newCacheArray;
			}
			// create cache entry
			var cache = new TypeInfoCache(type);
			cacheArray[cacheIndex] = cache;
			ObjectHeaderAccessor.SetIndex(type, cacheIndex);
			return cache;
		}

		/// <summary>
		/// Get cached information for FieldInfo
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static FieldInfoCache GetFieldInfoCache(FieldInfo fieldInfo) {
			// null check
			if (fieldInfo == null) {
				throw new ArgumentNullException("fieldInfo");
			}
			// for thread safe
			var cacheArray = _fieldInfoCacheArray;
			// get cache by index
			var cacheIndex = ObjectHeaderAccessor.GetIndex(fieldInfo);
			if (cacheIndex > 0 && cacheIndex < cacheArray.Length) {
				var cached = cacheArray[cacheIndex];
				if (cached != null) {
					// cache array may replaced by other thread,
					// it so we need create the cache entry again
					return cached;
				}
			}
			// get a new cache index
			if (cacheIndex == 0) {
				if (_fieldInfoCacheIndex > ObjectHeaderAccessor.MaxIndex) {
					return null;
				}
				cacheIndex = Interlocked.Increment(ref _fieldInfoCacheIndex);
				if (cacheIndex > ObjectHeaderAccessor.MaxIndex) {
					return null;
				}
			}
			// create new cache array if size not enough
			if (cacheIndex >= cacheArray.Length) {
				var newCacheArray = new FieldInfoCache[
					Math.Min(
						Math.Max(cacheArray.Length * 2, cacheIndex + 1),
						ObjectHeaderAccessor.MaxIndex + 1)];
				Array.Copy(cacheArray, newCacheArray, cacheArray.Length);
				cacheArray = newCacheArray;
				_fieldInfoCacheArray = newCacheArray;
			}
			// create cache entry
			var cache = new FieldInfoCache(fieldInfo);
			cacheArray[cacheIndex] = cache;
			ObjectHeaderAccessor.SetIndex(fieldInfo, cacheIndex);
			return cache;
		}

		/// <summary>
		/// Get cached information for PropertyInfo
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static PropertyInfoCache GetPropertyInfoCache(PropertyInfo propertyInfo) {
			// null check
			if (propertyInfo == null) {
				throw new ArgumentNullException("propertyInfo");
			}
			// for thread safe
			var cacheArray = _propertyInfoCacheArray;
			// get cache by index
			var cacheIndex = ObjectHeaderAccessor.GetIndex(propertyInfo);
			if (cacheIndex > 0 && cacheIndex < cacheArray.Length) {
				var cached = cacheArray[cacheIndex];
				if (cached != null) {
					// cache array may replaced by other thread,
					// it so we need create the cache entry again
					return cached;
				}
			}
			// get a new cache index
			if (cacheIndex == 0) {
				if (_propertyInfoCacheIndex > ObjectHeaderAccessor.MaxIndex) {
					return null;
				}
				cacheIndex = Interlocked.Increment(ref _propertyInfoCacheIndex);
				if (cacheIndex > ObjectHeaderAccessor.MaxIndex) {
					return null;
				}
			}
			// create new cache array if size not enough
			if (cacheIndex >= cacheArray.Length) {
				var newCacheArray = new PropertyInfoCache[
					Math.Min(
						Math.Max(cacheArray.Length * 2, cacheIndex + 1),
						ObjectHeaderAccessor.MaxIndex + 1)];
				Array.Copy(cacheArray, newCacheArray, cacheArray.Length);
				cacheArray = newCacheArray;
				_propertyInfoCacheArray = newCacheArray;
			}
			// create cache entry
			var cache = new PropertyInfoCache(propertyInfo);
			cacheArray[cacheIndex] = cache;
			ObjectHeaderAccessor.SetIndex(propertyInfo, cacheIndex);
			return cache;
		}

		/// <summary>
		/// Get cached information for MethodInfo
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static MethodInfoCache GetMethodInfoCache(MethodInfo methodInfo) {
			// null check
			if (methodInfo == null) {
				throw new ArgumentNullException("methodInfo");
			}
			// for thread safe
			var cacheArray = _methodInfoCacheArray;
			// get cache by index
			var cacheIndex = ObjectHeaderAccessor.GetIndex(methodInfo);
			if (cacheIndex > 0 && cacheIndex < cacheArray.Length) {
				var cached = cacheArray[cacheIndex];
				if (cached != null) {
					// cache array may replaced by other thread,
					// it so we need create the cache entry again
					return cached;
				}
			}
			// get a new cache index
			if (cacheIndex == 0) {
				if (_methodInfoCacheIndex > ObjectHeaderAccessor.MaxIndex) {
					return null;
				}
				cacheIndex = Interlocked.Increment(ref _methodInfoCacheIndex);
				if (cacheIndex > ObjectHeaderAccessor.MaxIndex) {
					return null;
				}
			}
			// create new cache array if size not enough
			if (cacheIndex >= cacheArray.Length) {
				var newCacheArray = new MethodInfoCache[
					Math.Min(
						Math.Max(cacheArray.Length * 2, cacheIndex + 1),
						ObjectHeaderAccessor.MaxIndex + 1)];
				Array.Copy(cacheArray, newCacheArray, cacheArray.Length);
				cacheArray = newCacheArray;
				_methodInfoCacheArray = newCacheArray;
			}
			// create cache entry
			var cache = new MethodInfoCache(methodInfo);
			cacheArray[cacheIndex] = cache;
			ObjectHeaderAccessor.SetIndex(methodInfo, cacheIndex);
			return cache;
		}

		/// <summary>
		/// Fast get all fields from type
		/// Notice: it will include all non-public and static fields
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FieldInfo[] FastGetFields(this Type type) {
			var cache = GetTypeInfoCache(type);
			if (cache != null) {
				return cache.AllFields;
			}
			return type.GetFields(TypeInfoCache.DefaultBindingFlags);
		}

		/// <summary>
		/// Fast get field from type
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FieldInfo FastGetField(this Type type, string name) {
			var cache = GetTypeInfoCache(type);
			if (cache != null) {
				var field = cache.GetField(name);
				if (field != null) {
					return field;
				}
			}
			return type.GetField(name, TypeInfoCache.DefaultBindingFlags);
		}

		/// <summary>
		/// Fast get value from field
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object FastGetValue(this FieldInfo fieldInfo, object instance) {
			var getter = GetFieldInfoCache(fieldInfo)?.Getter;
			if (getter != null) {
				return getter(instance);
			}
			return fieldInfo.GetValue(instance);
		}

		/// <summary>
		/// Fast set value to field
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FastSetValue(this FieldInfo fieldInfo, object instance, object value) {
			var setter = GetFieldInfoCache(fieldInfo)?.Setter;
			if (setter != null) {
				setter(instance, value);
			} else {
				fieldInfo.SetValue(instance, value);
			}
		}

		/// <summary>
		/// Fast get all properties from type
		/// Notice: it will include all non-public and static properties
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PropertyInfo[] FastGetProperties(this Type type) {
			var cache = GetTypeInfoCache(type);
			if (cache != null) {
				return cache.AllProperties;
			}
			return type.GetProperties(TypeInfoCache.DefaultBindingFlags);
		}

		/// <summary>
		/// Fast get property from type
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PropertyInfo FastGetProperty(this Type type, string name) {
			var cache = GetTypeInfoCache(type);
			if (cache != null) {
				var property = cache.GetProperty(name);
				if (property != null) {
					return property;
				}
			}
			return type.GetProperty(name, TypeInfoCache.DefaultBindingFlags);
		}

		/// <summary>
		/// Fast get value from property
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object FastGetValue(this PropertyInfo propertyInfo, object instance) {
			var getter = GetPropertyInfoCache(propertyInfo)?.Getter;
			if (getter != null) {
				return getter(instance);
			}
			return propertyInfo.GetValue(instance);
		}

		/// <summary>
		/// Fast set value to property
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value) {
			var setter = GetPropertyInfoCache(propertyInfo)?.Setter;
			if (setter != null) {
				setter(instance, value);
			} else {
				propertyInfo.SetValue(instance, value);
			}
		}

		/// <summary>
		/// Fast get all methods from type
		/// Notice: it will include all non-public and static methods
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo[] FastGetMethods(this Type type) {
			var cache = GetTypeInfoCache(type);
			if (cache != null) {
				return cache.AllMethods;
			}
			return type.GetMethods(TypeInfoCache.DefaultBindingFlags);
		}

		/// <summary>
		/// Fast get method from type
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo FastGetMethod(this Type type, string name) {
			var cache = GetTypeInfoCache(type);
			if (cache != null) {
				var method = cache.GetMethod(name);
				if (method != null) {
					return method;
				}
			}
			return type.GetMethod(name, TypeInfoCache.DefaultBindingFlags);
		}

		/// <summary>
		/// Fast invoke method
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object FastInvoke(
			this MethodInfo method, object instance, params object[] parameters) {
			var cache = GetMethodInfoCache(method);
			if (cache != null) {
				return cache.Invoker(instance, parameters);
			}
			return method.Invoke(instance, parameters);
		}
	}
}
