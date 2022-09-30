using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jobs.Aggregator.Aws.Services.Contracts
{
    public interface IS3Service
    {
        /// <returns>The file key</returns>
        Task<string> PutJsonObjectAsync(string bucketName, string key, string body, CancellationToken cancellationToken);
        Task<string> ReadObjectDataAsync(string bucketName, string key, CancellationToken cancellationToken);

        Task<IEnumerable<string>> ListObjectsKeysAsync(string bucketName, CancellationToken cancellationToken);
    }
}