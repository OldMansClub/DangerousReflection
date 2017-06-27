using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DangerousReflection.Tests {
	public class TestMethodReflection {
		class Example {
			public int voidMethodInvokeCount = 0;

			public void VoidMethod(string a) {
				Assert.Equal(a, "void method parameter");
				Interlocked.Increment(ref voidMethodInvokeCount);
			}

			public static string StaticMethod(string a, string b) {
				return a + b;
			}

			private static int PrivateStaticMethod() {
				return 123;
			}
		}

		[Fact]
		public void TestGetMethods() {
			var example = new Example();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var methods = exampleType.FastGetMethods();
				Assert.True(methods.Any(x => x.Name == "VoidMethod"));
				Assert.True(methods.Any(x => x.Name == "StaticMethod"));
				Assert.True(methods.Any(x => x.Name == "PrivateStaticMethod"));
			});
		}

		[Fact]
		public void TestGetMethod() {
			var example = new Example();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var methodVoid = exampleType.FastGetMethod("VoidMethod");
				var methodStatic = exampleType.FastGetMethod("StaticMethod");
				var methodPrivate = exampleType.FastGetMethod("PrivateStaticMethod");
				var methodNotExist = exampleType.FastGetMethod("NotExistMethod");
				Assert.NotEqual(null, methodVoid);
				Assert.NotEqual(null, methodStatic);
				Assert.NotEqual(null, methodPrivate);
				Assert.Equal(null, methodNotExist);
				Assert.Equal("VoidMethod", methodVoid.Name);
				Assert.Equal("StaticMethod", methodStatic.Name);
				Assert.Equal("PrivateStaticMethod", methodPrivate.Name);
			});
		}

		[Fact]
		public void TestInvoke() {
			var example = new Example();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var methodVoid = exampleType.FastGetMethod("VoidMethod");
				var methodStatic = exampleType.FastGetMethod("StaticMethod");
				var methodPrivate = exampleType.FastGetMethod("PrivateStaticMethod");
				var resultVoid = methodVoid.FastInvoke(example, "void method parameter");
				var resultStatic = methodStatic.FastInvoke(null, "a", "bc");
				var resultPrivate = methodPrivate.FastInvoke(null);
				Assert.Equal(null, resultVoid);
				Assert.Equal("abc", resultStatic);
				Assert.Equal(123, resultPrivate);
			});
			Assert.Equal(100, example.voidMethodInvokeCount);
		}
	}
}
