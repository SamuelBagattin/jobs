using System.Collections.Generic;

namespace Jobs.Aggregator.Core.FinalModels
{
    public record FinalJob
    {
        public string Id { get; init; }
        public IEnumerable<string> MainTechnologies { get; init; }

        public IEnumerable<string> SecondaryTechnologies { get; init; }

        public IEnumerable<FinalSite> Site { get; set; }

        public string JobTitle { get; set; }
        
        public JobStatistics Statistics { get; set; }
    }
}