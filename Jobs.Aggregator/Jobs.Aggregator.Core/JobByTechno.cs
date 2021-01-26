using System;
using System.Collections.Generic;

namespace Jobs.Aggregator.Core
{
    public class JobByTechno
    {
        public HashSet<TechnologiesEnum> Technologies { get; set; }
        public string Company { get; set; }
        public List<string> JobUrl { get; set; }
        
        public DateTime? LastSeen { get; set; }
        
        public string Site { get; set; }

        public override string ToString()
        {
            return $"{nameof(Technologies)}: {string.Join(',',Technologies)}, {nameof(Company)}: {Company}, {nameof(JobUrl)}: {JobUrl}, {nameof(LastSeen)}: {LastSeen}, {nameof(Site)}: {Site}";
        }
    }

}