using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jobs.Aggregator.Aws.Services.Contracts
{
    public interface IS3Service
    {
        Task PutObjectAsync(string bucketName, string key, string body);
        Task<string> ReadObjectDataAsync(string bucketName, string key);

        Task<IEnumerable<string>> ListObjectsKeysAsync(string bucketName);
    }
}