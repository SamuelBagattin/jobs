using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Jobs.Aggregator.Core.TransitionModels;

namespace Jobs.Aggregator.Core.FinalModels
{
    public class Response
    {
        public CompanyResponse Companies { get; set; }
        public TechnologiesResponse Technologies { get; set; }
    }

    public class TechnologiesResponse
    {
        public IEnumerable<TechnologyStatistics> Data { get; set; }
        public int TechnologiesCount => Data.Count();
    }

    public class TechnologyStatistics
    {
        public string TechnologyName { get; set; }
        public TechnologyStatisticsJob JobsWithMainTechnology { get; set; }
        public TechnologyStatisticsJob JobsWithSecondaryTechnology { get; set; }
        public TechnologyStatisticsCompany CompaniesWithSecondaryTechnologies { get; set; }
        public TechnologyStatisticsCompany CompaniesWithPrimaryTechnologies { get; set; }
    }

    public class TechnologyStatisticsCompany
    {
        [JsonIgnore]
        public IEnumerable<string> Ids { get; set; }
        public int Count => Ids.Count();
    }
    
    public class TechnologyStatisticsJob
    {
        [JsonIgnore]
        public IEnumerable<string> Ids { get; set; }
        public int Count => Ids.Count();
    }


    public class CompanyResponse
    {
        public IEnumerable<FinalCompany> Data { get; set; }
        public int Count => Data.Count();
    }
    public class FinalCompany
    {
        public string Id { get; set; }
        public IEnumerable<string> PrimaryTechnologies { get; set; }
        
        public IEnumerable<string> SecondaryTechnologies { get; set; }
        public string Company { get; set; }
        public IEnumerable<FinalJob> Jobs { get; set; }
    }

    public class FinalJob
    {
        public string Id { get; set; }
        public IEnumerable<string> PrimaryTechnologies { get; set; }
         
        public IEnumerable<string> SecondaryTechnologies { get; set; }

        public IEnumerable<FinalSite> Site { get; set; }
        
        public string Title { get; set; }
    }

    public class FinalSite
    {
        public string SiteName { get; set; }
        public string JobUrl { get; set; }
    }
}