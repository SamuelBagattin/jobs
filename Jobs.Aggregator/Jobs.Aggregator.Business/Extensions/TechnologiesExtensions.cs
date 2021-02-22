using System.Collections.Generic;
using System.Linq;
using Jobs.Aggregator.Core.Services;

namespace Jobs.Aggregator.Core.Extensions
{
    public static class TechnologiesExtensions
    {
        public static IEnumerable<string> ToDisplayNames(this IEnumerable<TechnologiesEnum> members) =>
            members.Select(TechnologiesService.GetTechnologyName);
        
        public static string ToDisplayName(this TechnologiesEnum member) =>
            TechnologiesService.GetTechnologyName(member);
    }
}