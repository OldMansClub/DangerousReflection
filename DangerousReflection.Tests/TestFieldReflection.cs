using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DangerousReflection.Tests {
	public class TestFieldReflection {
		class Example {
			public int a;
			public int b;
			public static int abc;
			string asd;
			static KeyValuePair<int, string> xxx;

			public string Asd { get => asd; set => asd = value; }
			public static KeyValuePair<int, string> Xxx { get => xxx; set => xxx = value; }
		}

		struct ValueExample {
			public int a;
			public int b;
			public static int abc;
			string asd;
			static KeyValuePair<int, string> xxx;

			public string Asd { get => asd; set => asd = value; }
			public static KeyValuePair<int, string> Xxx { get => xxx; set => xxx = value; }
		}

		class EmptyExample {

		}

		[Fact]
		public void TestGetFields() {
			var example = new Example();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var fields = exampleType.FastGetFields();
				Assert.True(fields.Any(x => x.Name == "a"));
				Assert.True(fields.Any(x => x.Name == "b"));
				Assert.True(fields.Any(x => x.Name == "abc"));
				Assert.True(fields.Any(x => x.Name == "asd"));
				Assert.True(fields.Any(x => x.Name == "xxx"));
			});
		}

		[Fact]
		public void TestGetField() {
			var example = new Example();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var fieldA = exampleType.FastGetField("a");
				var fieldB = exampleType.FastGetField("b");
				var fieldC = exampleType.FastGetField("c");
				var fieldAb = exampleType.FastGetField("ab");
				var fieldAbc = exampleType.FastGetField("abc");
				var fieldAsd = exampleType.FastGetField("asd");
				var fieldXxx = exampleType.FastGetField("xxx");
				var fieldXxxx = exampleType.FastGetField("xxxx");
				Assert.NotEqual(null, fieldA);
				Assert.NotEqual(null, fieldB);
				Assert.Equal(null, fieldC);
				Assert.Equal(null, fieldAb);
				Assert.NotEqual(null, fieldAbc);
				Assert.NotEqual(null, fieldAsd);
				Assert.NotEqual(null, fieldXxx);
				Assert.Equal(null, fieldXxxx);
				Assert.Equal("a", fieldA.Name);
				Assert.Equal("b", fieldB.Name);
				Assert.Equal("abc", fieldAbc.Name);
				Assert.Equal("asd", fieldAsd.Name);
				Assert.Equal("xxx", fieldXxx.Name);
			});
		}

		[Fact]
		public void TestEmptyGetField() {
			var example = new EmptyExample();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var fieldA = exampleType.FastGetField("a");
				Assert.Equal(null, fieldA);
			});
		}

		[Fact]
		public void TestFieldGetValue() {
			var example = new Example();
			example.a = 123;
			example.b = 321;
			Example.abc = 999;
			example.Asd = "example string";
			Example.Xxx = new KeyValuePair<int, string>(888, "example value");
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var fieldA = exampleType.FastGetField("a");
				var fieldB = exampleType.FastGetField("b");
				var fieldAbc = exampleType.FastGetField("abc");
				var fieldAsd = exampleType.FastGetField("asd");
				var fieldXxx = exampleType.FastGetField("xxx");
				Assert.Equal(123, fieldA.FastGetValue(example));
				Assert.Equal(321, fieldB.FastGetValue(example));
				Assert.Equal(999, fieldAbc.FastGetValue(null));
				Assert.Equal("example string", fieldAsd.FastGetValue(example));
				Assert.Equal(
					new KeyValuePair<int, string>(888, "example value"),
					fieldXxx.FastGetValue(null));
			});
		}

		[Fact]
		public void TestFieldSetValue() {
			var example = new Example();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var fieldA = exampleType.FastGetField("a");
				var fieldB = exampleType.FastGetField("b");
				var fieldAbc = exampleType.FastGetField("abc");
				var fieldAsd = exampleType.FastGetField("asd");
				var fieldXxx = exampleType.FastGetField("xxx");
				fieldA.FastSetValue(example, 123);
				fieldB.FastSetValue(example, 321);
				fieldAbc.FastSetValue(null, 999);
				fieldAsd.FastSetValue(example, "example string");
				fieldXxx.FastSetValue(null,
					new KeyValuePair<int, string>(888, "example value"));
			});
			Assert.Equal(123, example.a);
			Assert.Equal(321, example.b);
			Assert.Equal(999, Example.abc);
			Assert.Equal("example string", example.Asd);
			Assert.Equal(
				new KeyValuePair<int, string>(888, "example value"),
				Example.Xxx);
		}

		[Fact]
		public void TestValueTypeGetSetValue() {
			var example = (object)new ValueExample() { a = 0, b = 0, Asd = null };
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var fieldA = exampleType.FastGetField("a");
				var fieldB = exampleType.FastGetField("b");
				var fieldAbc = exampleType.FastGetField("abc");
				var fieldAsd = exampleType.FastGetField("asd");
				var fieldXxx = exampleType.FastGetField("xxx");
				fieldA.FastSetValue(example, 123);
				fieldB.FastSetValue(example, 321);
				fieldAbc.FastSetValue(null, 999);
				fieldAsd.FastSetValue(example, "example string");
				fieldXxx.FastSetValue(null,
					new KeyValuePair<int, string>(888, "example value"));
			});
			Parallel.For(0, 100, _ => {
				var fieldA = exampleType.FastGetField("a");
				var fieldB = exampleType.FastGetField("b");
				var fieldAbc = exampleType.FastGetField("abc");
				var fieldAsd = exampleType.FastGetField("asd");
				var fieldXxx = exampleType.FastGetField("xxx");
				Assert.Equal(123, fieldA.FastGetValue(example));
				Assert.Equal(321, fieldB.FastGetValue(example));
				Assert.Equal(999, fieldAbc.FastGetValue(null));
				Assert.Equal("example string", fieldAsd.FastGetValue(example));
				Assert.Equal(
					new KeyValuePair<int, string>(888, "example value"),
					fieldXxx.FastGetValue(null));
			});
		}
	}
}
