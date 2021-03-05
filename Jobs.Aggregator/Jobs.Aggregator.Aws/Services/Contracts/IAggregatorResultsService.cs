using System.Threading.Tasks;
using Jobs.Aggregator.Core.FinalModels;

namespace Jobs.Aggregator.Aws.Services.Contracts
{
    public interface IAggregatorResultsService
    {
        Task UploadAggregatedJobs(ResponseRoot body);
    }
}