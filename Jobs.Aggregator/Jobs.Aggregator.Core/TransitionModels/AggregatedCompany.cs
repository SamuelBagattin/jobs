using System;
using System.Collections.Generic;
using System.Linq;

namespace Jobs.Aggregator.Core.TransitionModels
{
    public record AggregatedCompany
    {
        public string Id { get; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);

        public HashSet<TechnologiesEnum> MainTechnologies =>
            Jobs.SelectMany(e => e.Value.MainTechnologies).ToHashSet();

        public HashSet<TechnologiesEnum> SecondaryTechnologies =>
            Jobs.SelectMany(e => e.Value.SecondaryTechnologies).ToHashSet();

        public string Company { get; set; }
        public Dictionary<string, JobByTechno> Jobs { get; set; }
    }
}