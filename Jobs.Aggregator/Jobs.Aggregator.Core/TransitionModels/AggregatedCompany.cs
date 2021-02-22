using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Jobs.Aggregator.Core.TransitionModels
{
    public class AggregatedCompany
    {

        public string Id { get; } = Guid.NewGuid().ToString();
        public HashSet<TechnologiesEnum> PrimaryTechnologies =>
            Jobs.SelectMany(e => e.Value.PrimaryTechnologies).ToHashSet();

        public HashSet<TechnologiesEnum> SecondaryTechnologies => Jobs.SelectMany(e => e.Value.SecondaryTechnologies).ToHashSet();

        public string Company { get; set; }
        public Dictionary<string, JobByTechno> Jobs { get; set; }
    }

    public class JobByTechno
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public HashSet<TechnologiesEnum> PrimaryTechnologies { get; set; }

        public HashSet<TechnologiesEnum> SecondaryTechnologies { get; set; }

        public Dictionary<string, string> SitesWithUrls { get; set; }

        public string Title { get; set; }
    }

    public class JobByTechnoWithCompany
    {
        public HashSet<TechnologiesEnum> PrimaryTechnologies { get; set; }

        public HashSet<TechnologiesEnum> SecondaryTechnologies { get; set; }

        public string Company { get; set; }

        public string Url { get; set; }

        public string Site { get; set; }

        public string Title { get; set; }
    }
}