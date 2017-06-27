using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DangerousReflection.Tests {
	public class TestPropertyReflection {
		class Example {
			public int a { get; set; }
			public int b { get; set; }
			public static int abc { get; set; }
			string asd { get; set; }
			static KeyValuePair<int, string> xxx { get; set; }

			public string Asd { get => asd; set => asd = value; }
			public static KeyValuePair<int, string> Xxx { get => xxx; set => xxx = value; }
		}

		struct ValueExample {
			public int a { get; set; }
			public int b { get; set; }
			public static int abc { get; set; }
			string asd { get; set; }
			static KeyValuePair<int, string> xxx { get; set; }

			public string Asd { get => asd; set => asd = value; }
			public static KeyValuePair<int, string> Xxx { get => xxx; set => xxx = value; }
		}

		class EmptyExample {

		}

		[Fact]
		public void TestGetProperties() {
			var example = new Example();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var properties = exampleType.FastGetProperties();
				Assert.True(properties.Any(x => x.Name == "a"));
				Assert.True(properties.Any(x => x.Name == "b"));
				Assert.True(properties.Any(x => x.Name == "abc"));
				Assert.True(properties.Any(x => x.Name == "asd"));
				Assert.True(properties.Any(x => x.Name == "xxx"));
			});
		}

		[Fact]
		public void TestGetProperty() {
			var example = new Example();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var propertyA = exampleType.FastGetProperty("a");
				var propertyB = exampleType.FastGetProperty("b");
				var propertyC = exampleType.FastGetProperty("c");
				var propertyAb = exampleType.FastGetProperty("ab");
				var propertyAbc = exampleType.FastGetProperty("abc");
				var propertyAsd = exampleType.FastGetProperty("asd");
				var propertyXxx = exampleType.FastGetProperty("xxx");
				var propertyXxxx = exampleType.FastGetProperty("xxxx");
				Assert.NotEqual(null, propertyA);
				Assert.NotEqual(null, propertyB);
				Assert.Equal(null, propertyC);
				Assert.Equal(null, propertyAb);
				Assert.NotEqual(null, propertyAbc);
				Assert.NotEqual(null, propertyAsd);
				Assert.NotEqual(null, propertyXxx);
				Assert.Equal(null, propertyXxxx);
				Assert.Equal("a", propertyA.Name);
				Assert.Equal("b", propertyB.Name);
				Assert.Equal("abc", propertyAbc.Name);
				Assert.Equal("asd", propertyAsd.Name);
				Assert.Equal("xxx", propertyXxx.Name);
			});
		}

		[Fact]
		public void TestEmptyGetProperty() {
			var example = new EmptyExample();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var propertyA = exampleType.FastGetProperty("a");
				Assert.Equal(null, propertyA);
			});
		}

		[Fact]
		public void TestPropertyGetValue() {
			var example = new Example();
			example.a = 123;
			example.b = 321;
			Example.abc = 999;
			example.Asd = "example string";
			Example.Xxx = new KeyValuePair<int, string>(888, "example value");
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var propertyA = exampleType.FastGetProperty("a");
				var propertyB = exampleType.FastGetProperty("b");
				var propertyAbc = exampleType.FastGetProperty("abc");
				var propertyAsd = exampleType.FastGetProperty("asd");
				var propertyXxx = exampleType.FastGetProperty("xxx");
				Assert.Equal(123, propertyA.FastGetValue(example));
				Assert.Equal(321, propertyB.FastGetValue(example));
				Assert.Equal(999, propertyAbc.FastGetValue(null));
				Assert.Equal("example string", propertyAsd.FastGetValue(example));
				Assert.Equal(
					new KeyValuePair<int, string>(888, "example value"),
					propertyXxx.FastGetValue(null));
			});
		}

		[Fact]
		public void TestPropertySetValue() {
			var example = new Example();
			var exampleType = example.GetType();
			Parallel.For(0, 100, _ => {
				var propertyA = exampleType.FastGetProperty("a");
				var propertyB = exampleType.FastGetProperty("b");
				var propertyAbc = exampleType.FastGetProperty("abc");
				var propertyAsd = exampleType.FastGetProperty("asd");
				var propertyXxx = exampleType.FastGetProperty("xxx");
				propertyA.FastSetValue(example, 123);
				propertyB.FastSetValue(example, 321);
				propertyAbc.FastSetValue(null, 999);
				propertyAsd.FastSetValue(example, "example string");
				propertyXxx.FastSetValue(null,
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
				var propertyA = exampleType.FastGetProperty("a");
				var propertyB = exampleType.FastGetProperty("b");
				var propertyAbc = exampleType.FastGetProperty("abc");
				var propertyAsd = exampleType.FastGetProperty("asd");
				var propertyXxx = exampleType.FastGetProperty("xxx");
				propertyA.FastSetValue(example, 123);
				propertyB.FastSetValue(example, 321);
				propertyAbc.FastSetValue(null, 999);
				propertyAsd.FastSetValue(example, "example string");
				propertyXxx.FastSetValue(null,
					new KeyValuePair<int, string>(888, "example value"));
			});
			Parallel.For(0, 100, _ => {
				var propertyA = exampleType.FastGetProperty("a");
				var propertyB = exampleType.FastGetProperty("b");
				var propertyAbc = exampleType.FastGetProperty("abc");
				var propertyAsd = exampleType.FastGetProperty("asd");
				var propertyXxx = exampleType.FastGetProperty("xxx");
				Assert.Equal(123, propertyA.FastGetValue(example));
				Assert.Equal(321, propertyB.FastGetValue(example));
				Assert.Equal(999, propertyAbc.FastGetValue(null));
				Assert.Equal("example string", propertyAsd.FastGetValue(example));
				Assert.Equal(
					new KeyValuePair<int, string>(888, "example value"),
					propertyXxx.FastGetValue(null));
			});
		}
	}
}
