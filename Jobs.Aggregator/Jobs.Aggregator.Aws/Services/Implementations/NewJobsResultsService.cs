using System.Collections.Generic;
using System.Text.Json;
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

        public Task UploadNewJobs(List<FinalCompany> newJobs)
        {
            return _s3Service.PutJsonObjectAsync(_awsConfigurationService.NewJobsBucketName, "index", JsonSerializer.Serialize(newJobs));
        }
    }
}