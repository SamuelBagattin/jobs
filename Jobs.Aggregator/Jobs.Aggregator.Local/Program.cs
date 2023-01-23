using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CloudFront;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Aws.Services.Implementations;
using Jobs.Aggregator.Core.Services.Contracts;
using Jobs.Aggregator.Core.Services.Implementations;
using Jobs.Aggregator.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHostBuilder CreateHostBuilder()
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) => services.AddTransient<ITechnologiesService, TechnologiesService>()
            .AddTransient<IAggregatorService, AggregatorService>()
            .AddTransient<IAggregatorResultsService, AggregatorResultsService>()
            .AddTransient<IScraperResultsService, ScraperResultsService>()
            .AddTransient<IS3Service, S3Service>()
            .AddTransient<ICloudfrontService, CloudfrontService>()
            .AddTransient<IAwsConfigurationService, AwsConfigurationService>()
            .AddTransient<IXXHash32, XXHash32>()
            .AddTransient<IIdService, IdService>()
            .AddTransient<INewJobsService, NewJobsService>()
            .AddTransient<INewJobsResultsService, NewJobsResultsService>()
            .AddAWSService<IAmazonS3>()
            .AddAWSService<IAmazonCloudFront>());
}

async Task<string> Handler(string input, ILambdaContext context)
{
    var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        Console.WriteLine("Canceling...");
        cts.Cancel();
        e.Cancel = true;
    };
    using var host = CreateHostBuilder().Build();
    await host.StartAsync(cts.Token);
    await host.Services.GetRequiredService<IAggregatorService>().Aggregate(cts.Token);
    return "hello";
}

var result = bool.TryParse(Environment.GetEnvironmentVariable("ON_LAMBDA").AsSpan(), out var onLambda);


    await Handler("dumb",null!);



