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

            var res = (await _scraperResultsService.GetAllScrapedJobs()).Select(j => new JobByTechnoWithCompany()
            {
                Company = j.Company,
                Technologies = TechnologiesService.GetTechnologies(new[] {j.Title, j.Description}).ToHashSet(),
                Url = j.Url,
                Site = j.Site,
                Title = j.Title,
            }).Aggregate(new Dictionary<string, AggregatedCompany>(), (acc, job) =>
            {
                // If company does not exists and if current job contains technologies
                if (!acc.ContainsKey(job.Company))
                {
                    if (job.Technologies.Count != 0)
                    {
                        acc[job.Company] = new AggregatedCompany
                        {
                            Company = job.Company,
                            Jobs = new Dictionary<string, JobByTechno>
                            {
                                {
                                    job.Title,
                                    new JobByTechno
                                    {
                                        SitesWithUrls = new Dictionary<string, string>{{job.Site,job.Url}},
                                        Technologies = job.Technologies,
                                        Title = job.Title
                                    }
                                }
                            }
                        };
                    }
                }
                else
                {
                    // If company already contains the job
                    if (acc[job.Company].Jobs.ContainsKey(job.Title))
                    {
                        foreach (var techno in job.Technologies)
                        {
                            acc[job.Company].Jobs[job.Title].Technologies.Add(techno);
                        }

                        if (!acc[job.Company].Jobs[job.Title].SitesWithUrls.ContainsKey(job.Site))
                        {
                            acc[job.Company].Jobs[job.Title].SitesWithUrls[job.Site] = job.Url;
                        }
                    }
                    // If company does not contains the job, and if job has technologies
                    else if(job.Technologies.Count != 0)
                    {
                        acc[job.Company].Jobs[job.Title] = new JobByTechno
                        {
                            Technologies = job.Technologies,
                            Title = job.Title,
                            SitesWithUrls = new Dictionary<string, string>
                            {
                                {job.Site, job.Url}
                            }
                        };
                    }
                }
                return acc;
            }, list => list.Select(e => e.Value)).Where(e => e.TechnologiesbyName.Count() != 0);
            File.Create("../../../../linkedin_aggregated.json").Close();
            await File.WriteAllTextAsync("../../../../linkedin_aggregated.json", JsonSerializer.Serialize(res));
            // await _fireStoreJobsService.InsertJobs(res.ToList());
        }
    }
}