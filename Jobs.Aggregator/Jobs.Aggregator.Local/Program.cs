﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CloudFront;
using Amazon.S3;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Aws.Services.Implementations;
using Jobs.Aggregator.Core.Services.Contracts;
using Jobs.Aggregator.Core.Services.Implementations;
using Jobs.Aggregator.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jobs.Aggregator.Local
{
    public static class Program
    {
        public static async Task Main()
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };
            using var host = CreateHostBuilder().Build();
            await host.StartAsync(cts.Token);
            await host.Services.GetRequiredService<IAggregatorService>().Aggregate(cts.Token);
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
                        .AddTransient<IXXHash32, XXHash32>()
                        .AddTransient<IIdService, IdService>()
                        .AddTransient<INewJobsService, NewJobsService>()
                        .AddTransient<INewJobsResultsService, NewJobsResultsService>()
                        .AddAWSService<IAmazonS3>()
                        .AddAWSService<IAmazonCloudFront>()
                );
        }
    }
}