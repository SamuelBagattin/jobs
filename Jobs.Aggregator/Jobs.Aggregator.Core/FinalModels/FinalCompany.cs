using System.Collections.Generic;

namespace Jobs.Aggregator.Core.FinalModels
{
    public record FinalCompany
    {
        public string Id { get; init; }
        public IEnumerable<string> MainTechnologies { get; init; }

        public IEnumerable<string> SecondaryTechnologies { get; init; }
        public string CompanyName { get; init; }
        public IEnumerable<FinalJob> Jobs { get; init; }
        
        public int JobCount { get; init; }
    }
}