using System.Threading.Tasks;
using Amazon.CloudFront;
using Amazon.S3;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Aws.Services.Implementations;
using Jobs.Aggregator.Core.Services.Contracts;
using Jobs.Aggregator.Core.Services.Implementations;
using Jobs.Aggregator.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jobs.Aggregator.Benchmark
{
    public static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<AggregatorBenchmark>();
        }


    }

    [MemoryDiagnoser(false)]
    public class AggregatorBenchmark
    {
        [Benchmark]
        public async Task RunAggregator()
        {
            await Local.Program.Main();
        }
    }
}


