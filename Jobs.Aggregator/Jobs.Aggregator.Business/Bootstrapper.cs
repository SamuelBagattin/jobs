using System;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Jobs.Aggregator.Aws;
using Jobs.Aggregator.Core.Services;
using Jobs.Aggregator.Firestore;
using Jobs.Aggregator.Postgres;
using Microsoft.Extensions.Logging;

namespace Jobs.Aggregator.Core
{
    public static class Bootstrapper
    {
        public static AggregatorService Init()
        {
            Console.WriteLine("Bootstrapping...");
            return new AggregatorService(new JobsRepository(new JobsContext()),
                new ScraperResultsService(new AmazonS3Client()));
        }
    }
}