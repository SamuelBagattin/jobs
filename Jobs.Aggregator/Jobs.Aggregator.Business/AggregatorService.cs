using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws;
using Jobs.Aggregator.Firestore;
using Jobs.Aggregator.Postgres;

namespace Jobs.Aggregator.Core
{
    public class AggregatorService
    {
        private readonly JobsRepository _jobsRepository;
        private readonly FireStoreJobsService _fireStoreJobsService;
        private readonly ScraperResultsService _scraperResultsService;

        
        public AggregatorService(JobsRepository jobsRepository, ScraperResultsService scraperResultsService)
        {
            _jobsRepository = jobsRepository;
            // _fireStoreJobsService = fireStoreJobsService;
            _scraperResultsService = scraperResultsService;
        }
        public async Task Aggregate()
        {
            Console.WriteLine("Aggregating Jobs");
            
            var res = (await _scraperResultsService.GetAllScrapedJobs()).Select(j => new JobByTechno
            {
                Company = j.Company,
                Technologies = TechnologiesService.GetTechnologies(new []{j.Title, j.Description}).ToHashSet(),
                JobUrl = new List<string>{j.Url},
                LastSeen = j.Scrapedate,
                Site = j.Site
            }).Aggregate(new Dictionary<string,JobByTechno>(), (acc, job) =>
            {
                if (!acc.ContainsKey(job.Company))
                {
                    acc[job.Company] = job;
                }
                else
                {
                    acc[job.Company].Technologies.UnionWith(job.Technologies);
                    acc[job.Company].JobUrl.Add(job.JobUrl[0]);
                    if (acc[job.Company].LastSeen.HasValue && acc[job.Company].LastSeen.Value < job.LastSeen)
                    {
                        acc[job.Company].LastSeen = job.LastSeen;
                    }
                }
                return acc;
            }, list => list.Select(e => e.Value)).Where(e => e.Technologies.Count != 0);
            File.Create("../../../../linkedin_aggregated.json").Close();
            await File.WriteAllTextAsync("../../../../linkedin_aggregated.json", JsonSerializer.Serialize(res));
            // await _fireStoreJobsService.InsertJobs(res.ToList());
        }
    }
}