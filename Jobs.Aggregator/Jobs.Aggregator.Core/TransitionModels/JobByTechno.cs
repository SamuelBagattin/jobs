using System;
using System.Collections.Generic;

namespace Jobs.Aggregator.Core.TransitionModels
{
    public record JobByTechno
    {
        public string Id { get; set; }
        public HashSet<TechnologiesEnum> MainTechnologies { get; set; }

        public HashSet<TechnologiesEnum> SecondaryTechnologies { get; set; }

        public Dictionary<string, string> SitesWithUrls { get; set; }

        public string Title { get; set; }
    }
}