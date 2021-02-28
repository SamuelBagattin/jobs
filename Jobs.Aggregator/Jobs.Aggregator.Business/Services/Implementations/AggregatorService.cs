using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Core.FinalModels;
using Jobs.Aggregator.Core.Services.Contracts;
using Jobs.Aggregator.Core.TransitionModels;
using Microsoft.Extensions.Logging;

namespace Jobs.Aggregator.Core.Services.Implementations
{
    public class AggregatorService : IAggregatorService
    {
        private readonly ILogger<AggregatorService> _logger;
        private readonly IScraperResultsService _scraperResultsService;
        private readonly ITechnologiesService _technologiesService;

        public AggregatorService(
            IScraperResultsService scraperResultsService,
            ITechnologiesService technologiesService,
            ILogger<AggregatorService> logger
        )
        {
            (_scraperResultsService, _technologiesService, _logger) =
                (scraperResultsService, technologiesService, logger);
        }

        public async Task Aggregate()
        {
            _logger.LogInformation("Aggregating jobs");

            var res = (await _scraperResultsService.GetAllScrapedJobs()).Select(j => new JobByTechnoWithCompany
                {
                    Company = j.Company,
                    PrimaryTechnologies = _technologiesService.GetTechnologies(j.Title).ToHashSet(),
                    SecondaryTechnologies = _technologiesService.GetTechnologies(j.Description).ToHashSet(),
                    Url = j.Url,
                    Site = j.Site,
                    Title = j.Title
                }).Aggregate(new Dictionary<string, AggregatedCompany>(), (acc, job) =>
                {
                    // If company does not exists and if current job contains technologies
                    if (!acc.ContainsKey(job.Company))
                    {
                        if (job.PrimaryTechnologies.Count != 0 || job.SecondaryTechnologies.Count != 0)
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
                    else
                    {
                        // If company already contains the job
                        if (acc[job.Company].Jobs.ContainsKey(job.Title))
                        {
                            foreach (var techno in job.PrimaryTechnologies)
                                acc[job.Company].Jobs[job.Title].PrimaryTechnologies.Add(techno);

                            foreach (var techno in job.SecondaryTechnologies)
                                acc[job.Company].Jobs[job.Title].SecondaryTechnologies.Add(techno);

                            if (!acc[job.Company].Jobs[job.Title].SitesWithUrls.ContainsKey(job.Site))
                                acc[job.Company].Jobs[job.Title].SitesWithUrls[job.Site] = job.Url;
                        }
                        // If company does not contains the job, and if job has technologies
                        else if ((job.PrimaryTechnologies.Count != 0) | (job.SecondaryTechnologies.Count != 0))
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
                        Id = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8),
                        Company = e.Company,
                        PrimaryTechnologies = e.PrimaryTechnologies.Select(_technologiesService.GetTechnologyName),
                        SecondaryTechnologies = e.SecondaryTechnologies.Select(_technologiesService.GetTechnologyName),
                        Jobs = e.Jobs.Select(job => new FinalJob
                        {
                            Title = job.Value.Title,
                            Id = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8),
                            PrimaryTechnologies =
                                job.Value.PrimaryTechnologies.Select(_technologiesService.GetTechnologyName),
                            SecondaryTechnologies =
                                job.Value.SecondaryTechnologies.Select(_technologiesService.GetTechnologyName),
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
                    Data = _technologiesService.GetAllTechnologies().Select(techno => new TechnologyStatistics
                    {
                        TechnologyName = _technologiesService.GetTechnologyName(techno),
                        CompaniesWithPrimaryTechnologies = new TechnologyStatisticsCompany
                        {
                            Ids = res.Where(cmp => cmp.PrimaryTechnologies.Contains(techno)).Select(cmp => cmp.Id)
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
                        }
                    })
                }
            };
            File.Create("../../../../linkedin_aggregated.json").Close();
            await File.WriteAllTextAsync("../../../../linkedin_aggregated.json", JsonSerializer.Serialize(res2));
            // await _fireStoreJobsService.InsertJobs(res.ToList());
        }
    }
}