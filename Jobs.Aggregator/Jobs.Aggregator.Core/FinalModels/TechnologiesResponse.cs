using System.Collections.Generic;
using System.Linq;

namespace Jobs.Aggregator.Core.FinalModels
{
    public record TechnologiesResponse
    {
        public IEnumerable<Technology> Technologies { get; set; }
    }
}