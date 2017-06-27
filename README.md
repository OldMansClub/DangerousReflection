# DangerousReflection

Ultra fast yet dangerous reflection extension, for now it only works on .Net Framework.

# How to use

- Install [DangerousReflection](https://www.nuget.org/packages/DangerousReflection) from nuget.
- Change `GetField`, `GetValue` to `FastGetField`, `FastGetValue`, and so on.

# How does it work

It use the bits in object header of `Type`, `FieldInfo`, `PropertyInfo` and `MethodInfo` as cache index.
So the access to the extra reflection information is O(1).
But it's dangerous because it may cause undefined behavior, if you want a stable fast reflection extension please see [FastReflection](https://www.nuget.org/packages/FastReflection) or [ZKWeb.Fork.FastReflection](https://www.nuget.org/packages/ZKWeb.Fork.FastReflection).

# How to avoid the problem

Don't use lock(obj) on `Type`, `FieldInfo`, `PropertyInfo` and `MethodInfo` instances.
Don't use this extension on dynamic type (the object may be moved during reflection operation by gc compaction).

# Benchmark

``` text
(Benchmark) BenchmarkFieldReflection.DefaultGetFields: 0.9063732s, GC: [152, 0, 0, 0]
(Benchmark) BenchmarkFieldReflection.FastGetFields: 0.0781336s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkFieldReflection.DefaultGetField: 1.2813868s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkFieldReflection.FastGetField: 0.6563236s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkFieldReflection.DefaultGetValue: 3.4378644s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkFieldReflection.FastGetValue: 0.5625276s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkFieldReflection.NativeGetValue: 0.0937602s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkFieldReflection.DefaultSetValue: 4.6879962s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkFieldReflection.FastSetValue: 0.5625876s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkFieldReflection.NativeSetValue: 0.0781411s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkPropertyReflection.DefaultGetProperties: 0.8751026s, GC: [152, 0, 0, 0]
(Benchmark) BenchmarkPropertyReflection.FastGetProperties: 0.0781287s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkPropertyReflection.DefaultGetProperty: 1.8126542s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkPropertyReflection.FastGetProperty: 0.71886s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkPropertyReflection.DefaultGetValue: 6.0943915s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkPropertyReflection.FastGetValue: 0.8750928s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkPropertyReflection.NativeGetValue: 0.0937639s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkPropertyReflection.DefaultSetValue: 10.0323104s, GC: [1017, 1, 0, 0]
(Benchmark) BenchmarkPropertyReflection.FastSetValue: 0.8750924s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkPropertyReflection.NativeSetValue: 0.1093863s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkMethodReflection.DefaultGetMethods: 1.7658138s, GC: [559, 0, 0, 0]
(Benchmark) BenchmarkMethodReflection.FastGetMethods: 0.0625066s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkMethodReflection.DefaultGetMethod: 1.2657581s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkMethodReflection.FastGetMethod: 0.3750413s, GC: [0, 0, 0, 0]
(Benchmark) BenchmarkMethodReflection.DefaultInvoke: 12.5325786s, GC: [1144, 1, 0, 0]
(Benchmark) BenchmarkMethodReflection.FastInvoke: 1.7345587s, GC: [508, 0, 0, 0]
(Benchmark) BenchmarkMethodReflection.NativeInvoke: 0.8438397s, GC: [508, 0, 0, 0]
```

# License

MIT License<br/>
Copyright © 2017 303248153@github<br/>
If you have any license issue please contact 303248153@qq.com.<br/>
