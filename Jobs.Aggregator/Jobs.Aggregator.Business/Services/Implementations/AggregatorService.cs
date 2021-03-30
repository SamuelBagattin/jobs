using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws.Configuration;
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
        private readonly IAggregatorResultsService _aggregatorResultsService;
        private readonly IAwsConfigurationService _awsConfigurationService;

        public AggregatorService(
            IScraperResultsService scraperResultsService,
            ITechnologiesService technologiesService,
            ILogger<AggregatorService> logger, IAggregatorResultsService aggregatorResultsService,
            IAwsConfigurationService awsConfigurationService)
        {
            _aggregatorResultsService = aggregatorResultsService;
            _awsConfigurationService = awsConfigurationService;
            (_scraperResultsService, _technologiesService, _logger) =
                (scraperResultsService, technologiesService, logger);
        }

        public async Task Aggregate()
        {
            _logger.LogInformation("Aggregating jobs");

            var res = (await _scraperResultsService.GetAllScrapedJobs()).Select(j => new JobByTechnoWithCompany
                {
                    Company = j.Company,
                    MainTechnologies = _technologiesService.GetTechnologies(j.Title).ToHashSet(),
                    SecondaryTechnologies = _technologiesService.GetTechnologies(j.Description).ToHashSet(),
                    Url = j.Url,
                    Site = j.Site,
                    Title = j.Title
                }).Aggregate(new Dictionary<string, AggregatedCompany>(), (acc, job) =>
                {
                    // If company does not exists and if current job contains technologies
                    if (!acc.ContainsKey(job.Company))
                    {
                        if (job.MainTechnologies.Count != 0 || job.SecondaryTechnologies.Count != 0)
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
                                            MainTechnologies = job.MainTechnologies,
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
                            foreach (var techno in job.MainTechnologies)
                                acc[job.Company].Jobs[job.Title].MainTechnologies.Add(techno);

                            foreach (var techno in job.SecondaryTechnologies)
                                acc[job.Company].Jobs[job.Title].SecondaryTechnologies.Add(techno);

                            if (!acc[job.Company].Jobs[job.Title].SitesWithUrls.ContainsKey(job.Site))
                                acc[job.Company].Jobs[job.Title].SitesWithUrls[job.Site] = job.Url;
                        }
                        // If company does not contains the job, and if job has technologies
                        else if ((job.MainTechnologies.Count != 0) | (job.SecondaryTechnologies.Count != 0))
                        {
                            acc[job.Company].Jobs[job.Title] = new JobByTechno
                            {
                                MainTechnologies = job.MainTechnologies,
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
                .Where(e => e.SecondaryTechnologies.Count != 0 || e.MainTechnologies.Count != 0).ToHashSet();

            var res2 = new ResponseRoot
            {
                Companies = new CompanyResponse
                {
                    Companies = res.Select(e => new FinalCompany
                    {
                        Id = e.Id,
                        CompanyName = e.Company,
                        MainTechnologies = e.MainTechnologies.Select(_technologiesService.GetTechnologyName),
                        SecondaryTechnologies = e.SecondaryTechnologies.Select(_technologiesService.GetTechnologyName),
                        Jobs = e.Jobs.Select(job => new FinalJob
                        {
                            JobTitle = job.Value.Title,
                            Id = job.Value.Id,
                            MainTechnologies =
                                job.Value.MainTechnologies.Select(_technologiesService.GetTechnologyName),
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
                    Technologies = _technologiesService.GetAllTechnologies().Select(techno => new TechnologyStatistics
                    {
                        TechnologyName = _technologiesService.GetTechnologyName(techno),
                        CompaniesWithMainTechnologies = new TechnologyStatisticsCompany
                        {
                            Ids = res.Where(cmp => cmp.MainTechnologies.Contains(techno)).Select(cmp => cmp.Id)
                        },
                        CompaniesWithSecondaryTechnologies = new TechnologyStatisticsCompany
                        {
                            Ids = res.Where(cmp => cmp.SecondaryTechnologies.Contains(techno)).Select(cmp => cmp.Id)
                        },
                        JobsWithMainTechnology = new TechnologyStatisticsJob
                        {
                            Ids = res.SelectMany(cmp => cmp.Jobs)
                                .Where(job => job.Value.MainTechnologies.Contains(techno))
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
            if (_awsConfigurationService.WriteResultsToLocal)
            {
                File.Create("../../../../index.json").Close();
                await File.WriteAllTextAsync("../../../../index.json", JsonSerializer.Serialize(res2));
            }

            if (_awsConfigurationService.UploadResults)
            {
                await _aggregatorResultsService.UploadAggregatedJobs(res2);
            }

            ;
        }
    }
}