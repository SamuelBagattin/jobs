using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Jobs.Aggregator.Aws.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Jobs.Aggregator.Aws.Services.Implementations
{
    public class S3Service : IS3Service
    {
        private readonly ILogger<S3Service> _logger;
        private readonly IAmazonS3 _s3Client;

        public S3Service(IAmazonS3 s3Client, ILogger<S3Service> logger)
        {
            _s3Client = s3Client;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> ListObjectsKeysAsync(string bucketName)
        {
            return (await _s3Client.ListObjectsAsync(bucketName)).S3Objects.Select(e => e.Key);
        }

        public async Task PutJsonObjectAsync(string bucketName, string key, string body)
        {
            if (bucketName is null)
            {
                throw new Exception("");
            }
            
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    ContentType = "application/json",
                    ContentBody = body
                };
                await _s3Client.PutObjectAsync(request);
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                _logger.LogError("Error encountered ***. Message:'{0}' when adding object", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Unknown encountered on server. Message:'{0}' when adding object", e.Message);
            }
        }

        public async Task<string> ReadObjectDataAsync(string bucketName, string key)
        {
            var responseBody = "";
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };
                using GetObjectResponse response = await _s3Client.GetObjectAsync(request);
                await using var responseStream = response.ResponseStream;
                using var reader = new StreamReader(responseStream);
                var contentType = response.Headers["Content-Type"];
                _logger.LogInformation("Content type: {0}", contentType);

                responseBody = await reader.ReadToEndAsync(); // Now you process the response body.
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                _logger.LogError("Error encountered ***. Message:'{0}' when reading object", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            }

            return responseBody;
        }
    }
}