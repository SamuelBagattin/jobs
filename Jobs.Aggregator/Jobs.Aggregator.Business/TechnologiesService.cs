using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Jobs.Aggregator.Core
{
    public static class TechnologiesService
    {
        
        private static readonly RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

        
        private static readonly Dictionary<TechnologiesEnum, Regex> TechnosRegexes =
            new Dictionary<TechnologiesEnum, Regex>
            {
                {TechnologiesEnum.Java, new Regex("java|jee|boot|spring", _options)},
                {TechnologiesEnum.Dotnet, new Regex(@"c#|\.net", _options)},
                {TechnologiesEnum.Nodejs, new Regex(@"node", _options)},
                {TechnologiesEnum.Vuejs, new Regex(@" vue |vuejs", _options)},
                {TechnologiesEnum.React, new Regex(@"react", _options)},
                {TechnologiesEnum.Angular, new Regex(@"angular", _options)},
                {TechnologiesEnum.Golang, new Regex(@" go |golang", _options)},
                {TechnologiesEnum.Php, new Regex(@"php|symfony|laravel", _options)},
                {TechnologiesEnum.Cpp, new Regex(@"c\+\+", _options)},
                {TechnologiesEnum.C, new Regex(@" c ", _options)},
                {TechnologiesEnum.Devops, new Regex(@"devops", _options)},
                {TechnologiesEnum.Cloud, new Regex(@" cloud ", _options)},
            };


        public static IEnumerable<TechnologiesEnum> GetTechnologies(string title) =>
            TechnosRegexes.Where(
                    keyValuePair => TechnosRegexes[keyValuePair.Key].IsMatch(title))
                .Select(keyValuePair => keyValuePair.Key).ToList();
    }

}