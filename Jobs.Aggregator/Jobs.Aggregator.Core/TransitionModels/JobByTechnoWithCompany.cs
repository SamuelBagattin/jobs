using System.Collections.Generic;

namespace Jobs.Aggregator.Core.TransitionModels
{
    public class JobByTechnoWithCompany
    {
        public HashSet<TechnologiesEnum> MainTechnologies { get; set; }

        public HashSet<TechnologiesEnum> SecondaryTechnologies { get; set; }

        public string Company { get; set; }

        public string Url { get; set; }

        public string Site { get; set; }

        public string Title { get; set; }
    }
}