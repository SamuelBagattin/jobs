using System.Collections.Generic;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class FinalJob
    {
        public string Id { get; set; }
        public IEnumerable<string> MainTechnologies { get; set; }

        public IEnumerable<string> SecondaryTechnologies { get; set; }

        public IEnumerable<FinalSite> Site { get; set; }

        public string JobTitle { get; set; }
    }
}