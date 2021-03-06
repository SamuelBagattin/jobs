using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class TechnologyStatisticsJob
    {
        public IEnumerable<string> Ids { get; set; }

        public int Count => Ids.Count();
    }
}