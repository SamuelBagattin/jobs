using System;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class ResponseRoot
    {
        public DateTime LastUpdatedAt => DateTime.Now;
        public CompanyResponse Companies { get; set; }
        public TechnologiesResponse Technologies { get; set; }
    }
}