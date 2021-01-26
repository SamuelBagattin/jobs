using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Jobs.Aggregator.Core;

namespace Jobs.Aggregator.Firestore
{
    public class FireStoreJobsService
    {
        private readonly FirestoreDb _db;

        public FireStoreJobsService(FirestoreDb db)
        {
            _db = db;
        }

        public async Task InsertJobs(List<JobByTechno> jobs)
        {
            Console.WriteLine($"Inserting jobs {jobs.Count}into firestore");
            var collection = _db.Collection("jobs");
            foreach (var job in jobs)
            {
                if (job.LastSeen != null)
                    await collection.AddAsync(new
                    {
                        job.Company,
                        job.Site,
                        Technologies = job.Technologies.Select(e => Enum.GetName(typeof(TechnologiesEnum), e)),
                        job.JobUrl,
                        LastSeen = DateTime.SpecifyKind(job.LastSeen.Value, DateTimeKind.Utc)
                    });
            }
        }
    }
}