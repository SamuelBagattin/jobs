using System.Collections.Generic;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class FinalCompany
    {
        public string Id { get; set; }
        public IEnumerable<string> MainTechnologies { get; set; }

        public IEnumerable<string> SecondaryTechnologies { get; set; }
        public string CompanyName { get; set; }
        public IEnumerable<FinalJob> Jobs { get; set; }
    }
}