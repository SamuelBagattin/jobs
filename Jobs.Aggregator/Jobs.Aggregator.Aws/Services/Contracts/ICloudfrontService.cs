using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jobs.Aggregator.Aws.Services.Contracts
{
    public interface ICloudfrontService
    {
        Task CreateInvalidationByPath(string distributionId, IEnumerable<string> paths, CancellationToken cancellationToken);
    }
}