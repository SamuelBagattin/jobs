using System.Threading.Tasks;
using Amazon.S3;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Aws.Services.Implementations;
using Jobs.Aggregator.Core.Services.Contracts;
using Jobs.Aggregator.Core.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jobs.Aggregator.Local
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            await host.StartAsync();
            await host.Services.GetRequiredService<IAggregatorService>().Aggregate();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services
                        .AddTransient<ITechnologiesService, TechnologiesService>()
                        .AddTransient<IAggregatorService, AggregatorService>()
                        .AddTransient<IAggregatorResultsService, AggregatorResultsService>()
                        .AddTransient<IScraperResultsService, ScraperResultsService>()
                        .AddTransient<IS3Service, S3Service>()
                        .AddTransient<IS3ConfigurationService, S3ConfigurationService>()
                        .AddAWSService<IAmazonS3>()
                );
        }
    }
}