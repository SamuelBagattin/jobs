using System.Collections.Generic;
using System.Linq;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class CompanyResponse
    {
        public IEnumerable<FinalCompany> Companies { get; set; }
        public int Count => Companies.Count();
    }
}