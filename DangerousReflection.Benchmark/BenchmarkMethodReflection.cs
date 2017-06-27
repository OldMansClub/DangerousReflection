using JustBenchmark;
using System;
using System.Reflection;

namespace DangerousReflection.Benchmark {
	public class BenchmarkMethodReflection {
		private volatile object _result;
		private Example _obj;
		private Type _type;
		private MethodInfo _methodCombine;
		private object[] _methodParameters;
		public const int GetMethodsLoopCount = 1000;
		public const int GetMethodLoopCount = 1000;
		public const int InvokeLoopCount = 5000;

		class Example {
			public int Add(int a, int b) {
				return a + b;
			}

			public string Combine(string a, string b) {
				return a + b;
			}
		}

		public BenchmarkMethodReflection() {
			_obj = new Example();
			_type = _obj.GetType();
			_methodCombine = _type.GetMethod("Combine");
			_methodParameters = new[] { "a", "b" };
		}

		[Benchmark]
		public void DefaultGetMethods() {
			for (var x = 0; x < GetMethodsLoopCount; ++x) {
				_result = _type.GetMethods();
			}
		}

		[Benchmark]
		public void FastGetMethods() {
			for (var x = 0; x < GetMethodsLoopCount; ++x) {
				_result = _type.FastGetMethods();
			}
		}

		[Benchmark]
		public void DefaultGetMethod() {
			for (var x = 0; x < GetMethodLoopCount; ++x) {
				_result = _type.GetMethod("Add");
				_result = _type.GetMethod("Combine");
			}
		}

		[Benchmark]
		public void FastGetMethod() {
			for (var x = 0; x < GetMethodLoopCount; ++x) {
				_result = _type.FastGetMethod("Add");
				_result = _type.FastGetMethod("Combine");
			}
		}

		[Benchmark]
		public void DefaultInvoke() {
			for (var x = 0; x < InvokeLoopCount; ++x) {
				_result = _methodCombine.Invoke(_obj, _methodParameters);
			}
		}

		[Benchmark]
		public void FastInvoke() {
			for (var x = 0; x < InvokeLoopCount; ++x) {
				_result = _methodCombine.FastInvoke(_obj, _methodParameters);
			}
		}

		[Benchmark]
		public void NativeInvoke() {
			for (var x = 0; x < InvokeLoopCount; ++x) {
				_result = _obj.Combine("a", "b");
			}
		}
	}
}
