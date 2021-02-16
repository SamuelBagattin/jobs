using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace Jobs.Aggregator.Aws
{
    public class ScraperResultsService
    {
        private readonly AmazonS3Client _s3Client;

        public ScraperResultsService(AmazonS3Client s3Client)
        {
            _s3Client = s3Client;
        }

        private async Task<string> ReadObjectDataAsync(string bucketName, string key)
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
                using (var reader = new StreamReader(responseStream))
                {
                    var title =
                        response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    var contentType = response.Headers["Content-Type"];
                    Console.WriteLine("Object metadata, Title: {0}", title);
                    Console.WriteLine("Content type: {0}", contentType);

                    responseBody = await reader.ReadToEndAsync(); // Now you process the response body.
                }
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            }

            return responseBody;
        }

        public async Task<IEnumerable<Job>> GetAllScrapedJobs()
        {
            var objectList = await _s3Client.ListObjectsAsync("jobs-scraper-results");
            var test = objectList.S3Objects.Select(async e =>
            {
                var jsonData = await ReadObjectDataAsync(e.BucketName, e.Key);
                return JsonSerializer.Deserialize<Job[]>(jsonData);
            });
            var truc = await Task.WhenAll(test);
            return truc.SelectMany(e => e);

        }
    }
}