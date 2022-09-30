using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Jobs.Aggregator.Aws.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Jobs.Aggregator.Aws.Services.Implementations;

public class S3Service : IS3Service
{
    private readonly ILogger<S3Service> _logger;
    private readonly IAmazonS3 _s3Client;

    public S3Service(IAmazonS3 s3Client, ILogger<S3Service> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
    }

    public async Task<IEnumerable<string>> ListObjectsKeysAsync(string bucketName,
        CancellationToken cancellationToken)
    {
        string continuationToken = "";
        bool isTruncated;
        var allObjects = new List<S3Object>();
        do
        {
            _logger.LogInformation($"Retrieving keys from {bucketName}. ContinuationToken : {continuationToken}");
            var res = await _s3Client.ListObjectsAsync(new ListObjectsRequest
            {
                BucketName = bucketName,
                Marker = continuationToken
            }, cancellationToken);
            continuationToken = res.NextMarker;
            allObjects.AddRange(res.S3Objects);
            isTruncated = res.IsTruncated;
        } while (isTruncated);

        return allObjects.Select(e => e.Key);
    }

    public async Task<string> PutJsonObjectAsync(string bucketName, string key, string body,
        CancellationToken cancellationToken)
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
            await _s3Client.PutObjectAsync(request, cancellationToken);
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

        return key;
    }

    public async Task<string> ReadObjectDataAsync(string bucketName, string key,
        CancellationToken cancellationToken)
    {
        var responseBody = "";
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };
            using GetObjectResponse response = await _s3Client.GetObjectAsync(request, cancellationToken);
            await using var responseStream = response.ResponseStream;
            using var reader = new StreamReader(responseStream);

            responseBody = await reader.ReadToEndAsync(); // Now you process the response body.
        }
        catch (Exception e)
        {
            _logger.LogError(JsonSerializer.Serialize(new
            {
                Message = $"Error encountered ***. Message: {e.Message} when reading object", e.StackTrace
            }));
            throw;
        }

        return responseBody;
    }
}