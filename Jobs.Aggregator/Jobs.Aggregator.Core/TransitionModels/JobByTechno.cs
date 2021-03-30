using System;
using System.Collections.Generic;

namespace Jobs.Aggregator.Core.TransitionModels
{
    public class JobByTechno
    {
        public string Id { get; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
        public HashSet<TechnologiesEnum> MainTechnologies { get; set; }

        public HashSet<TechnologiesEnum> SecondaryTechnologies { get; set; }

        public Dictionary<string, string> SitesWithUrls { get; set; }

        public string Title { get; set; }
    }
}