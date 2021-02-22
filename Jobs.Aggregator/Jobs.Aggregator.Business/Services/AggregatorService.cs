using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws;
using Jobs.Aggregator.Core.Extensions;
using Jobs.Aggregator.Core.FinalModels;
using Jobs.Aggregator.Core.TransitionModels;
using Jobs.Aggregator.Firestore;
using Jobs.Aggregator.Postgres;

namespace Jobs.Aggregator.Core.Services
{
    public class AggregatorService
    {
        private readonly JobsRepository _jobsRepository;
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

            var res = (await _scraperResultsService.GetAllScrapedJobs()).Select(j => new JobByTechnoWithCompany
                {
                    Company = j.Company,
                    PrimaryTechnologies = TechnologiesService.GetTechnologies(j.Title).ToHashSet(),
                    SecondaryTechnologies = TechnologiesService.GetTechnologies(j.Description).ToHashSet(),
                    Url = j.Url,
                    Site = j.Site,
                    Title = j.Title,
                }).Aggregate(new Dictionary<string, AggregatedCompany>(), (acc, job) =>
                {
                    // If company does not exists and if current job contains technologies
                    if (!acc.ContainsKey(job.Company))
                    {
                        if (job.PrimaryTechnologies.Count != 0 || job.SecondaryTechnologies.Count != 0)
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
                                            SitesWithUrls = new Dictionary<string, string> {{job.Site, job.Url}},
                                            PrimaryTechnologies = job.PrimaryTechnologies,
                                            SecondaryTechnologies = job.SecondaryTechnologies,
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
                            foreach (var techno in job.PrimaryTechnologies)
                            {
                                acc[job.Company].Jobs[job.Title].PrimaryTechnologies.Add(techno);
                            }

                            foreach (var techno in job.SecondaryTechnologies)
                            {
                                acc[job.Company].Jobs[job.Title].SecondaryTechnologies.Add(techno);
                            }

                            if (!acc[job.Company].Jobs[job.Title].SitesWithUrls.ContainsKey(job.Site))
                            {
                                acc[job.Company].Jobs[job.Title].SitesWithUrls[job.Site] = job.Url;
                            }
                        }
                        // If company does not contains the job, and if job has technologies
                        else if (job.PrimaryTechnologies.Count != 0 | job.SecondaryTechnologies.Count != 0)
                        {
                            acc[job.Company].Jobs[job.Title] = new JobByTechno
                            {
                                PrimaryTechnologies = job.PrimaryTechnologies,
                                SecondaryTechnologies = job.SecondaryTechnologies,
                                Title = job.Title,
                                SitesWithUrls = new Dictionary<string, string>
                                {
                                    {job.Site, job.Url}
                                }
                            };
                        }
                    }

                    return acc;
                }, list => list.Select(e => e.Value))
                .Where(e => e.SecondaryTechnologies.Count() != 0 || e.PrimaryTechnologies.Count() != 0).ToHashSet();

            var res2 = new Response
            {
                Companies = new CompanyResponse
                {
                    Data = res.Select(e => new FinalCompany
                    {
                        Id = Guid.NewGuid().ToString(),
                        Company = e.Company,
                        PrimaryTechnologies = e.PrimaryTechnologies.ToDisplayNames(),
                        SecondaryTechnologies = e.SecondaryTechnologies.ToDisplayNames(),
                        Jobs = e.Jobs.Select(job => new FinalJob
                        {
                            Title = job.Value.Title,
                            Id = Guid.NewGuid().ToString(),
                            PrimaryTechnologies = job.Value.PrimaryTechnologies.ToDisplayNames(),
                            SecondaryTechnologies = job.Value.SecondaryTechnologies.ToDisplayNames(),
                            Site = job.Value.SitesWithUrls.Select(site => new FinalSite
                            {
                                JobUrl = site.Value,
                                SiteName = site.Key
                            })
                        })
                    })
                },
                Technologies = new TechnologiesResponse
                {
                    Data = TechnologiesService.GetAllTechnologies().Select(techno => new TechnologyStatistics
                    {
                        TechnologyName = techno.ToDisplayName(),
                        CompaniesWithPrimaryTechnologies = new TechnologyStatisticsCompany
                        {
                            Ids = res.Where(cmp => cmp.PrimaryTechnologies.Contains(techno)).Select(cmp => cmp.Id),
                        },
                        CompaniesWithSecondaryTechnologies = new TechnologyStatisticsCompany
                        {
                            Ids = res.Where(cmp => cmp.SecondaryTechnologies.Contains(techno)).Select(cmp => cmp.Id)
                        },
                        JobsWithMainTechnology = new TechnologyStatisticsJob
                        {
                            Ids = res.SelectMany(cmp => cmp.Jobs)
                                .Where(job => job.Value.PrimaryTechnologies.Contains(techno))
                                .Select(job => job.Value.Id)
                        },
                        JobsWithSecondaryTechnology = new TechnologyStatisticsJob
                        {
                            Ids = res.SelectMany(cmp => cmp.Jobs)
                                .Where(job => job.Value.SecondaryTechnologies.Contains(techno))
                                .Select(job => job.Value.Id)
                        },
                    })
                }
            };
            File.Create("../../../../linkedin_aggregated.json").Close();
            await File.WriteAllTextAsync("../../../../linkedin_aggregated.json", JsonSerializer.Serialize(res2));
            // await _fireStoreJobsService.InsertJobs(res.ToList());
        }
    }
}