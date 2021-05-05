using System.Collections.Generic;
using Jobs.Aggregator.Core.FinalModels;
using Jobs.Aggregator.Core.TransitionModels;

namespace Jobs.Aggregator.Core.Services.Implementations
{
    public interface ITechnologiesAggregatorService
    {
        IEnumerable<TechnologyStatistics> GetTechnologiesStatistics(ICollection<AggregatedCompany> res);
    }
}