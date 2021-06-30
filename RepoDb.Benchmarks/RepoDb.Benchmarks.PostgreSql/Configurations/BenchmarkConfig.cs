using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;

namespace RepoDb.Benchmarks.PostgreSql.Configurations
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddLogger(ConsoleLogger.Default);
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);

            AddColumn(new ORMColum());
            AddColumn(TargetMethodColumn.Method);
            AddColumn(StatisticColumn.Mean);
            AddColumn(StatisticColumn.StdDev);
            AddColumn(StatisticColumn.Error);
            AddColumn(BaselineRatioColumn.RatioMean);
            AddColumn(StatisticColumn.Min);
            AddColumn(StatisticColumn.Max);

            AddColumnProvider(DefaultColumnProviders.Metrics);

            var job = Job.ShortRun
                .WithRuntime(CoreRuntime.Core31)
                .WithLaunchCount(DefaultConstants.DefaultLaunchCount)
                .WithWarmupCount(DefaultConstants.DefaultWarmupCount)
                .WithUnrollFactor(DefaultConstants.DefaultUnrollFactor)
                .WithIterationCount(DefaultConstants.DefaultIterationCount);

            AddJob(job);

            Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
            Options |= ConfigOptions.JoinSummary | ConfigOptions.StopOnFirstError;
        }
    }
}