using System.Threading.Tasks;

namespace Jobs.Aggregator.Core.Services.Contracts
{
    public interface IAggregatorService
    {
        Task Aggregate();
    }
}