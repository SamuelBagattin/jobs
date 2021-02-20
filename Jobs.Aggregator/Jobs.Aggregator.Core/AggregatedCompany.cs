using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Jobs.Aggregator.Core
{
    public class AggregatedCompany
    {

        public IEnumerable<string?> TechnologiesbyName
        {
            get
            {
                return Jobs.SelectMany(e => e.Value.TechnologiesbyName).ToHashSet();
            }
        }

        public string Company { get; set; }
        public Dictionary<string,JobByTechno> Jobs { get; set; }
    }

    public class JobByTechno
    {
        [JsonIgnore]
        public HashSet<TechnologiesEnum> Technologies { get; set; }
        
        public IEnumerable<string?> TechnologiesbyName => Technologies.Select(e => Enum.GetName(typeof(TechnologiesEnum), e));

        public Dictionary<string, string> SitesWithUrls { get; set; }
        
        public string Title { get; set; }
    }

    public class JobByTechnoWithCompany
    {
        [JsonIgnore]
        public HashSet<TechnologiesEnum> Technologies { get; set; }
        
        public string Company { get; set; }
        
        public IEnumerable<string?> TechnologiesbyName => Technologies.Select(e => Enum.GetName(typeof(TechnologiesEnum), e));

        public string Url { get; set; }
        
        public string Site { get; set; }
        
        public string Title { get; set; }
    }

}