using System.Collections.Generic;
using System.Linq;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class CompanyResponse
    {
        public IEnumerable<FinalCompany> Data { get; set; }
        public int Count => Data.Count();
    }
}