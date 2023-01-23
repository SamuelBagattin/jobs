using System.Text.RegularExpressions;

namespace Jobs.Aggregator.Core.Models
{
    public record TechnologyData
    {
        public Regex Regex { get; init; }

        public string DisplayName { get; init; }
    }
}