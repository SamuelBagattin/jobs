using System.Threading.Tasks;

namespace Jobs.Aggregator.Aws.Services.Contracts
{
    public interface ICloudfrontService
    {
        Task CreateInvalidationByPath(string distributionId, string path);
    }
}