using System.Collections.Generic;
using System.Linq;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class TechnologiesResponse
    {
        public IEnumerable<TechnologyStatistics> Technologies { get; set; }
        public int TechnologiesCount => Technologies.Count();
    }
}