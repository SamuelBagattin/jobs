using System;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
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
            var firestoreBuilder = new FirestoreClientBuilder
            {
                // TODO
                JsonCredentials = @""
            };
            return new AggregatorService(new JobsRepository(new JobsContext()), new FireStoreJobsService(FirestoreDb.Create("jobs-scraper-31230", firestoreBuilder.Build())));
        }
    }
}
