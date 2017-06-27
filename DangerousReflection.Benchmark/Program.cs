namespace DangerousReflection.Benchmark {
	class Program {
		static void Main(string[] args) {
			var benchmark = new JustBenchmark.JustBenchmark();
			benchmark.Run(new BenchmarkFieldReflection());
			benchmark.Run(new BenchmarkPropertyReflection());
			benchmark.Run(new BenchmarkMethodReflection());
		}
	}
}
