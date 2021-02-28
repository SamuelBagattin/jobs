using System.Collections.Generic;
using System.Linq;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class TechnologiesResponse
    {
        public IEnumerable<TechnologyStatistics> Data { get; set; }
        public int TechnologiesCount => Data.Count();
    }
}