using System.Threading.Tasks;
using Amazon.CloudFront;
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
        private static async Task Main()
        {
            using var host = CreateHostBuilder().Build();
            await host.StartAsync();
            await host.Services.GetRequiredService<IAggregatorService>().Aggregate();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    services
                        .AddTransient<ITechnologiesService, TechnologiesService>()
                        .AddTransient<IAggregatorService, AggregatorService>()
                        .AddTransient<IAggregatorResultsService, AggregatorResultsService>()
                        .AddTransient<IScraperResultsService, ScraperResultsService>()
                        .AddTransient<IS3Service, S3Service>()
                        .AddTransient<ICloudfrontService, CloudfrontService>()
                        .AddTransient<IAwsConfigurationService, AwsConfigurationService>()
                        .AddTransient<ITechnologiesAggregatorService, TechnologiesAggregatorService>()
                        .AddAWSService<IAmazonS3>()
                        .AddAWSService<IAmazonCloudFront>()
                );
        }
    }
}