using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async Task CreateInvalidationByPath(string distributionId, IEnumerable<string> paths, CancellationToken cancellationToken)
        {
            var enumerable = paths.ToList();
            _logger.LogInformation($"Creating invalidation for {distributionId}: {string.Join(", ", enumerable)}");
            var invalidationRequest = new CreateInvalidationRequest
            {
                DistributionId = distributionId,
                InvalidationBatch = new InvalidationBatch
                {
                    Paths = new Paths
                    {
                        Items = enumerable.ToList(),
                        Quantity = enumerable.Count
                    },
                    CallerReference = DateTime.Now.ToString("yyyyMMddHHmmssffff")
                }
            };
            await _amazonCloudFront.CreateInvalidationAsync(invalidationRequest, cancellationToken);
        }
    }
}