using System.Text.RegularExpressions;

namespace Jobs.Aggregator.Core.Models
{
    public class TechnologyData
    {
        public Regex Regex { get; set; }

        public string DisplayName { get; set; }
    }
}