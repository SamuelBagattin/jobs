using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Jobs.Aggregator.Aws.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Jobs.Aggregator.Aws.Services.Implementations
{
    public class CloudfrontService : ICloudfrontService
    {
        private readonly IAmazonCloudFront _amazonCloudFront;
        private readonly ILogger<CloudfrontService> _logger;

        public CloudfrontService(IAmazonCloudFront amazonCloudFront, ILogger<CloudfrontService> logger)
        {
            _amazonCloudFront = amazonCloudFront;
            _logger = logger;
        }

        public async Task CreateInvalidationByPath(string distributionId, string path)
        {
            _logger.LogInformation($"Creating invalidation for {distributionId}: {path}");
            var invalidationRequest = new CreateInvalidationRequest
            {
                DistributionId = distributionId,
                InvalidationBatch = new InvalidationBatch
                {
                    Paths = new Paths
                    {
                        Items = new List<string>{path},
                        Quantity = 1
                    },
                    CallerReference = DateTime.Now.ToString("yyyyMMddHHmmssffff")
                }
            };
            await _amazonCloudFront.CreateInvalidationAsync(invalidationRequest);
        }
    }
}