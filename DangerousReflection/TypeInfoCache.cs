using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DangerousReflection {
	/// <summary>
	/// Cache for TypeInfo
	/// </summary>
	internal class TypeInfoCache {
		public const BindingFlags DefaultBindingFlags = (BindingFlags.Public |
			BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
		private const int HashMask = 255;
		public FieldInfo[] AllFields => _allFields;
		public PropertyInfo[] AllProperties => _allProperties;
		public MethodInfo[] AllMethods => _allMethods;
		private FieldInfo[] _allFields;
		private Dictionary<string, FieldInfo> _fieldsIndex;
		private FieldInfo[] _fieldsIndexByLength;
		private FieldInfo[] _fieldsIndexByHash;
		private PropertyInfo[] _allProperties;
		private Dictionary<string, PropertyInfo> _propertiesIndex;
		private PropertyInfo[] _propertiesIndexByLength;
		private PropertyInfo[] _propertiesIndexByHash;
		private MethodInfo[] _allMethods;
		private Dictionary<string, MethodInfo> _methodsIndex;
		private MethodInfo[] _methodsIndexByLength;
		private MethodInfo[] _methodsIndexByHash;

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="type"></param>
		public TypeInfoCache(Type type) {
			// setup fields index
			_allFields = type.GetFields(DefaultBindingFlags);
			_fieldsIndex = _allFields.ToDictionary(x => x.Name);
			var maxFieldNameLength = _allFields.Max(x => (int?)x.Name.Length) ?? 0;
			_fieldsIndexByLength = new FieldInfo[maxFieldNameLength];
			_fieldsIndexByHash = new FieldInfo[HashMask];
			foreach (var field in _allFields) {
				var lenIndex = field.Name.Length - 1;
				var hash = field.Name.GetHashCode() & HashMask;
				if (_fieldsIndexByLength[lenIndex] == null) {
					_fieldsIndexByLength[lenIndex] = field;
				}
				if (_fieldsIndexByHash[hash] == null) {
					_fieldsIndexByHash[hash] = field;
				}
			}
			// setup properties index
			_allProperties = type.GetProperties(DefaultBindingFlags);
			_propertiesIndex = _allProperties.ToDictionary(x => x.Name);
			var maxPropertyNameLength = _allProperties.Max(x => (int?)x.Name.Length) ?? 0;
			_propertiesIndexByLength = new PropertyInfo[maxPropertyNameLength];
			_propertiesIndexByHash = new PropertyInfo[HashMask];
			foreach (var property in _allProperties) {
				var lenIndex = property.Name.Length - 1;
				var hash = property.Name.GetHashCode() & HashMask;
				if (_propertiesIndexByLength[lenIndex] == null) {
					_propertiesIndexByLength[lenIndex] = property;
				}
				if (_propertiesIndexByHash[hash] == null) {
					_propertiesIndexByHash[hash] = property;
				}
			}
			// setup methods index
			_allMethods = type.GetMethods(DefaultBindingFlags)
				.Where(x => !x.IsSpecialName).ToArray();
			_methodsIndex = _allMethods.ToDictionary(x => x.Name);
			var maxMethodNameLength = _allMethods.Max(x => (int?)x.Name.Length) ?? 0;
			_methodsIndexByLength = new MethodInfo[maxMethodNameLength];
			_methodsIndexByHash = new MethodInfo[HashMask];
			foreach (var method in _allMethods) {
				var lenIndex = method.Name.Length - 1;
				var hash = method.Name.GetHashCode() & HashMask;
				if (_methodsIndexByLength[lenIndex] == null) {
					_methodsIndexByLength[lenIndex] = method;
				}
				if (_methodsIndexByHash[hash] == null) {
					_methodsIndexByHash[hash] = method;
				}
			}
		}

		/// <summary>
		/// Get field with given name
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FieldInfo GetField(string name) {
			// use length index
			var lenIndex = name.Length - 1;
			if (lenIndex < 0 || lenIndex >= _fieldsIndexByLength.Length) {
				return null;
			}
			var field = _fieldsIndexByLength[lenIndex];
			if (field != null && field.Name == name) {
				return field;
			}
			// use hash index
			var hash = name.GetHashCode() & HashMask;
			field = _fieldsIndexByHash[hash];
			if (field == null) {
				return null;
			} else if (field.Name == name) {
				return field;
			}
			// fallback
			if (_fieldsIndex.TryGetValue(name, out field)) {
				return field;
			}
			return null;
		}

		/// <summary>
		/// Get property with given name
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PropertyInfo GetProperty(string name) {
			// use length index
			var lenIndex = name.Length - 1;
			if (lenIndex < 0 || lenIndex >= _propertiesIndexByLength.Length) {
				return null;
			}
			var property = _propertiesIndexByLength[lenIndex];
			if (property != null && property.Name == name) {
				return property;
			}
			// use hash index
			var hash = name.GetHashCode() & HashMask;
			property = _propertiesIndexByHash[hash];
			if (property == null) {
				return null;
			} else if (property.Name == name) {
				return property;
			}
			// fallback
			if (_propertiesIndex.TryGetValue(name, out property)) {
				return property;
			}
			return null;
		}

		/// <summary>
		/// Get method with given name
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public MethodInfo GetMethod(string name) {
			// use length index
			var lenIndex = name.Length - 1;
			if (lenIndex < 0 || lenIndex >= _methodsIndexByLength.Length) {
				return null;
			}
			var method = _methodsIndexByLength[lenIndex];
			if (method != null && method.Name == name) {
				return method;
			}
			// use hash index
			var hash = name.GetHashCode() & HashMask;
			method = _methodsIndexByHash[hash];
			if (method == null) {
				return null;
			} else if (method.Name == name) {
				return method;
			}
			// fallback
			if (_methodsIndex.TryGetValue(name, out method)) {
				return method;
			}
			return null;
		}
	}
}
