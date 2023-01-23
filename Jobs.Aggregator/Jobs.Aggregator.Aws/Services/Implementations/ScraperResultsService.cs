using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Models;
using Jobs.Aggregator.Aws.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Jobs.Aggregator.Aws.Services.Implementations
{
    public class ScraperResultsService : IScraperResultsService
    {
        private readonly ILogger<ScraperResultsService> _logger;
        private readonly IAwsConfigurationService _iawsConfigurationService;
        private readonly IS3Service _s3Service;

        private const string LocalCachePath = "/tmp/jobs_aggregator_cache";

        public ScraperResultsService(ILogger<ScraperResultsService> logger, IS3Service s3Service,
            IAwsConfigurationService iawsConfigurationService)
        {
            _logger = logger;
            _s3Service = s3Service;
            _iawsConfigurationService = iawsConfigurationService;
        }


        public async Task<IEnumerable<Job>> GetAllScrapedJobs(CancellationToken cancellationToken)
        {
            var objectList = await _s3Service.ListObjectsKeysAsync(_iawsConfigurationService.SourceDataBucketName, cancellationToken);
            var enumerable = objectList.ToList();
            _logger.LogInformation("Number of files to download : {0}", enumerable.Count);
            var semaphore = new SemaphoreSlim(100);
            if (_iawsConfigurationService.WriteResultsToLocal && !Directory.Exists(LocalCachePath))
            {
                Directory.CreateDirectory(LocalCachePath);
            }
            var test = enumerable.Select(async e =>
            {
                await semaphore.WaitAsync(cancellationToken);

                try
                {
                    string jsonData;
                    var currentLocalFilePath = Path.Combine(LocalCachePath, HttpUtility.UrlEncode(e));
                    if (_iawsConfigurationService.WriteResultsToLocal && File.Exists(currentLocalFilePath))
                    {
                        jsonData = await File.ReadAllTextAsync(currentLocalFilePath, cancellationToken);
                        _logger.LogInformation("Using local cache");
                    }
                    else
                    {
                        jsonData = await _s3Service.ReadObjectDataAsync(_iawsConfigurationService.SourceDataBucketName, e, cancellationToken);
                    }

                    if (!File.Exists(currentLocalFilePath) && _iawsConfigurationService.WriteResultsToLocal)
                    {
                        await File.WriteAllTextAsync(currentLocalFilePath, jsonData, cancellationToken);
                    }
                    
                    return JsonSerializer.Deserialize<Job[]>(jsonData);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            var truc = await Task.WhenAll(test);
            _logger.LogInformation("Got the files");
            return truc.Where(e => e != null).SelectMany(e => e);
        }
    }
}