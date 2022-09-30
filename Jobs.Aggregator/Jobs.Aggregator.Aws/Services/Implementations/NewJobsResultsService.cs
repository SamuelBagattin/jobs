using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Core.FinalModels;

namespace Jobs.Aggregator.Aws.Services.Implementations
{
    public class NewJobsResultsService : INewJobsResultsService
    {
        private readonly IS3Service _s3Service;
        private readonly IAwsConfigurationService _awsConfigurationService;

        public NewJobsResultsService(IS3Service s3Service, IAwsConfigurationService awsConfigurationService)
        {
            _s3Service = s3Service;
            _awsConfigurationService = awsConfigurationService;
        }

        public Task UploadNewJobs(List<FinalCompany> newJobs, CancellationToken cancellationToken)
        {
            if (_awsConfigurationService.UploadResults)
                return _s3Service.PutJsonObjectAsync(_awsConfigurationService.NewJobsBucketName, "index", JsonSerializer.Serialize(newJobs), cancellationToken);
            
            if (!_awsConfigurationService.WriteResultsToLocal) return Task.CompletedTask;
            
            Directory.CreateDirectory(_awsConfigurationService.LocalNewJobsResultsPath);
            return File.WriteAllTextAsync(
                $"{Path.Combine(_awsConfigurationService.LocalNewJobsResultsPath, "newjobs_index.json")}",
                JsonSerializer.Serialize(newJobs), cancellationToken);

        }
    }
}