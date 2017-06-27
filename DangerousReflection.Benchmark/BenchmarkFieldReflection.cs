using JustBenchmark;
using System;
using System.Reflection;

namespace DangerousReflection.Benchmark {
	public class BenchmarkFieldReflection {
		private volatile object _result;
		private Example _obj;
		private Type _type;
		private FieldInfo _fieldAbc;
		public const int GetFieldsLoopCount = 1000;
		public const int GetFieldLoopCount = 1000;
		public const int AccessValueLoopCount = 5000;

		class Example {
			public int a;
			public int b;
			public string abc;
		}

		public BenchmarkFieldReflection() {
			_obj = new Example() { a = 123, b = 321, abc = "abc" };
			_type = _obj.GetType();
			_fieldAbc = _type.GetField("abc");
		}

		[Benchmark]
		public void DefaultGetFields() {
			for (var x = 0; x < GetFieldsLoopCount; ++x) {
				_result = _type.GetFields();
			}
		}

		[Benchmark]
		public void FastGetFields() {
			for (var x = 0; x < GetFieldsLoopCount; ++x) {
				_result = _type.FastGetFields();
			}
		}

		[Benchmark]
		public void DefaultGetField() {
			for (var x = 0; x < GetFieldLoopCount; ++x) {
				_result = _type.GetField("a");
				_result = _type.GetField("b");
				_result = _type.GetField("abc");
			}
		}

		[Benchmark]
		public void FastGetField() {
			for (var x = 0; x < GetFieldLoopCount; ++x) {
				_result = _type.FastGetField("a");
				_result = _type.FastGetField("b");
				_result = _type.FastGetField("abc");
			}
		}

		[Benchmark]
		public void DefaultGetValue() {
			for (var x = 0; x < AccessValueLoopCount; ++x) {
				_result = _fieldAbc.GetValue(_obj);
			}
		}

		[Benchmark]
		public void FastGetValue() {
			for (var x = 0; x < AccessValueLoopCount; ++x) {
				_result = _fieldAbc.FastGetValue(_obj);
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
				_fieldAbc.SetValue(_obj, "abc");
			}
		}

		[Benchmark]
		public void FastSetValue() {
			for (var x = 0; x < AccessValueLoopCount; ++x) {
				_fieldAbc.FastSetValue(_obj, "abc");
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
