using System.Collections.Generic;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class FinalJob
    {
        public string Id { get; set; }
        public IEnumerable<string> PrimaryTechnologies { get; set; }

        public IEnumerable<string> SecondaryTechnologies { get; set; }

        public IEnumerable<FinalSite> Site { get; set; }

        public string Title { get; set; }
    }
}