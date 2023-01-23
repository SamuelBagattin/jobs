using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Jobs.Aggregator.Core.FinalModels
{
    public record TechnologyStatisticsJob
    {
        public IEnumerable<string> Ids { get; init; }

        public int Count => Ids.Count();
    }
}