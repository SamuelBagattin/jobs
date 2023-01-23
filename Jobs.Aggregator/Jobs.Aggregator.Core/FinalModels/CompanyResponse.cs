using System.Collections.Generic;
using System.Linq;

namespace Jobs.Aggregator.Core.FinalModels
{
    public record CompanyResponse
    {
        public IEnumerable<FinalCompany> Companies { get; init; }
        public int Count { get; init; }
    }
}