using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Core.FinalModels;
using Jobs.Aggregator.Core.Services.Contracts;
using Jobs.Aggregator.Core.TransitionModels;
using Microsoft.Extensions.Logging;

namespace Jobs.Aggregator.Core.Services.Implementations;

public class AggregatorService : IAggregatorService
{
    private readonly ILogger<AggregatorService> _logger;
    private readonly IScraperResultsService _scraperResultsService;
    private readonly ITechnologiesService _technologiesService;
    private readonly IAggregatorResultsService _aggregatorResultsService;
    private readonly IAwsConfigurationService _awsConfigurationService;
    private readonly IIdService _idService;
    private readonly INewJobsResultsService _newJobsResultsService;
    private readonly INewJobsService _newJobsService;

    public AggregatorService(
        IScraperResultsService scraperResultsService,
        ITechnologiesService technologiesService,
        ILogger<AggregatorService> logger, IAggregatorResultsService aggregatorResultsService,
        IAwsConfigurationService awsConfigurationService, IIdService idService,
        INewJobsResultsService newJobsResultsService, INewJobsService newJobsService)
    {
        _aggregatorResultsService = aggregatorResultsService;
        _awsConfigurationService = awsConfigurationService;
        _idService = idService;
        _newJobsResultsService = newJobsResultsService;
        _newJobsService = newJobsService;
        (_scraperResultsService, _technologiesService, _logger) =
            (scraperResultsService, technologiesService, logger);
    }

    public async Task Aggregate(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Aggregating jobs");

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var scrapedJobs = (await _scraperResultsService.GetAllScrapedJobs(cancellationToken)).ToArray();
        stopwatch.Stop();
        _logger.LogInformation("GetAllScrapedJobs : {} seconds", stopwatch.Elapsed.TotalSeconds);

        stopwatch.Reset();
        stopwatch.Start();

        var deduplicatedJobs = scrapedJobs.DistinctBy(e => e.Url);

        stopwatch.Stop();
        _logger.LogInformation("deduplication : {} seconds", stopwatch.Elapsed.TotalSeconds);

        stopwatch.Reset();
        stopwatch.Start();
        var jobsWithAnalysedTechnologies = deduplicatedJobs.Select(j => new JobByTechnoWithCompany
        {
            Company = j.Company,
            MainTechnologies = _technologiesService.GetTechnologies(j.Title).ToHashSet(),
            SecondaryTechnologies = _technologiesService.GetTechnologies(j.Description).ToHashSet(),
            Url = j.Url,
            Site = j.Site,
            Title = j.Title
        }).Where(_technologiesService.isItJob);
        stopwatch.Stop();
        _logger.LogInformation("jobsWithAnalysedTechnologies : {} seconds", stopwatch.Elapsed.TotalSeconds);
        stopwatch.Reset();
        stopwatch.Start();


        //var aggregated = jobsWithAnalysedTechnologies.Aggregate(new Dictionary<string, AggregatedCompany>(), AggregateCompaniesAndJobs);

        var aggregated = jobsWithAnalysedTechnologies.AsParallel().GroupBy(e => e.Company, (key, jobs) =>
        {
            var jobsByTitle = jobs.GroupBy(e => e.Title);
            var finalJobs = jobsByTitle.Select(e =>
            {
                var allJobPostings = e.Select(f => new FinalSite
                {
                    JobUrl = f.Url,
                    SiteName = f.Site
                }).ToArray();
                return new FinalJob
                {
                    Id = _idService.GetJobId(key, e.Key).ToString(),
                    Statistics = new JobStatistics
                    {
                        Occurences = allJobPostings.Length,
                    },
                    Site = allJobPostings.GroupBy(f => f.SiteName).Select(f => f.Last()),
                    JobTitle = e.Key,
                    MainTechnologies = e.SelectMany(f => f.MainTechnologies).Distinct()
                        .Select(_technologiesService.GetTechnologyName),
                    SecondaryTechnologies = e.SelectMany(f => f.MainTechnologies).Distinct()
                        .Select(_technologiesService.GetTechnologyName)
                };
            }).ToArray();
            return new FinalCompany
            {
                Id = string.Join("",key.Select(e => char.IsLetterOrDigit(e) || new[]{'!','-','_','*','\'','(',')'}.Contains(e) ? e : '-')).Trim('-').Replace("---", "-").Replace("--","-").ToLower(),
                CompanyName = key,
                MainTechnologies = finalJobs.SelectMany(e => e.MainTechnologies).Distinct(),
                SecondaryTechnologies = finalJobs.SelectMany(e => e.SecondaryTechnologies).Distinct(),
                Jobs = finalJobs,
                JobCount = finalJobs.Length
            };
        }).OrderBy(e => e.JobCount).ToArray();

        stopwatch.Stop();
        _logger.LogInformation("aggregated : {} seconds", stopwatch.Elapsed.TotalSeconds);


        stopwatch.Reset();
        stopwatch.Start();

        var res2 = new ResponseRoot
        {
            Companies = new CompanyResponse
            {
                Companies = aggregated,
                Count = aggregated.Length
            },
            // Technologies = new TechnologiesResponse
            // {
            //     Technologies = 
            // }
        };
        if (_awsConfigurationService.WriteResultsToLocal)
        {
            File.Create("../../../../index.json").Close();
            await File.WriteAllTextAsync("../../../../index.json", JsonSerializer.Serialize(res2), cancellationToken);
        }

        stopwatch.Stop();
        _logger.LogInformation("statistics and serialization : {} seconds", stopwatch.Elapsed.TotalSeconds);
        stopwatch.Reset();
        stopwatch.Start();

        var previousJobs = await _aggregatorResultsService.GetLastUploadedAggregatedJobs(cancellationToken);
        var newJobs = _newJobsService.GetNewJobs(previousJobs, res2);
        await _newJobsResultsService.UploadNewJobs(newJobs, cancellationToken);

        await _aggregatorResultsService.UploadAggregatedJobs(res2, cancellationToken);

        stopwatch.Stop();
        _logger.LogInformation("upload : {} seconds", stopwatch.Elapsed.TotalSeconds);
        stopwatch.Reset();
        stopwatch.Start();
    }
}