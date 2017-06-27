using JustBenchmark;
using System;
using System.Reflection;

namespace DangerousReflection.Benchmark {
	public class BenchmarkPropertyReflection {
		private volatile object _result;
		private Example _obj;
		private Type _type;
		private PropertyInfo _propertyAbc;
		public const int GetPropertiesLoopCount = 1000;
		public const int GetPropertyLoopCount = 1000;
		public const int AccessValueLoopCount = 5000;

		class Example {
			public int a { get; set; }
			public int b { get; set; }
			public string abc { get; set; }
		}

		public BenchmarkPropertyReflection() {
			_obj = new Example() { a = 123, b = 321, abc = "abc" };
			_type = _obj.GetType();
			_propertyAbc = _type.GetProperty("abc");
		}

		[Benchmark]
		public void DefaultGetProperties() {
			for (var x = 0; x < GetPropertiesLoopCount; ++x) {
				_result = _type.GetProperties();
			}
		}

		[Benchmark]
		public void FastGetProperties() {
			for (var x = 0; x < GetPropertiesLoopCount; ++x) {
				_result = _type.FastGetProperties();
			}
		}

		[Benchmark]
		public void DefaultGetProperty() {
			for (var x = 0; x < GetPropertyLoopCount; ++x) {
				_result = _type.GetProperty("a");
				_result = _type.GetProperty("b");
				_result = _type.GetProperty("abc");
			}
		}

		[Benchmark]
		public void FastGetProperty() {
			for (var x = 0; x < GetPropertyLoopCount; ++x) {
				_result = _type.FastGetProperty("a");
				_result = _type.FastGetProperty("b");
				_result = _type.FastGetProperty("abc");
			}
		}

		[Benchmark]
		public void DefaultGetValue() {
			for (var x = 0; x < AccessValueLoopCount; ++x) {
				_result = _propertyAbc.GetValue(_obj);
			}
		}

		[Benchmark]
		public void FastGetValue() {
			for (var x = 0; x < AccessValueLoopCount; ++x) {
				_result = _propertyAbc.FastGetValue(_obj);
			}
		}

		[Benchmark]
		public void NativeGetValue() {
			for (var x = 0; x < AccessValueLoopCount; ++x) {
				_result = _obj.abc;
			}
		}

		[Benchmark]
		public void DefaultSetValue() {
			for (var x = 0; x < AccessValueLoopCount; ++x) {
				_propertyAbc.SetValue(_obj, "abc");
			}
		}

		[Benchmark]
		public void FastSetValue() {
			for (var x = 0; x < AccessValueLoopCount; ++x) {
				_propertyAbc.FastSetValue(_obj, "abc");
			}
		}

		[Benchmark]
		public void NativeSetValue() {
			for (var x = 0; x < AccessValueLoopCount; ++x) {
				_obj.abc = "abc";
			}
		}
	}
}
