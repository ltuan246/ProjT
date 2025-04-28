namespace KISS.QueryBuilder.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<FluentSqlBuilderBenchmarks>();
    }
}
