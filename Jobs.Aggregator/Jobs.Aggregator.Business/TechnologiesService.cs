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
                {TechnologiesEnum.Embedded, new Regex(@" syst(e|è)mes? embarqués? ", _options)},
                {TechnologiesEnum.Android, new Regex(@" android ", _options)},
                {TechnologiesEnum.Ios, new Regex(@" ios |swift", _options)},
                {TechnologiesEnum.Arduino, new Regex(@"arduino", _options)},
                {TechnologiesEnum.Tensorflow, new Regex(@"tensorflow", _options)},
                {TechnologiesEnum.Javascript, new Regex(@"javascript", _options)},
                {TechnologiesEnum.Typescript, new Regex(@"typescript", _options)},
                {TechnologiesEnum.Python, new Regex(@"python", _options)},
                {TechnologiesEnum.Hardware, new Regex(@"harware", _options)},
                {TechnologiesEnum.Iot, new Regex(@" iot | internet of things", _options)},
                {TechnologiesEnum.Sql, new Regex(@"sql", _options)},
                {TechnologiesEnum.PlSql, new Regex(@"pl ?/ ?sql | pl ?sql", _options)},
                {TechnologiesEnum.TSql, new Regex(@"t-?sql", _options)},
                {TechnologiesEnum.Aws, new Regex(@"aws|amazon web services", _options)},
                {TechnologiesEnum.Gcp, new Regex(@"gcp|google cloud", _options)},
                {TechnologiesEnum.Azure, new Regex(@"azure", _options)},
                {TechnologiesEnum.Jenkins, new Regex(@"jenkins", _options)},
                {TechnologiesEnum.Teamcity, new Regex(@"teamcity", _options)},
            };


        public static IEnumerable<TechnologiesEnum> GetTechnologies(string title) =>
            TechnosRegexes.Where(
                    keyValuePair => TechnosRegexes[keyValuePair.Key].IsMatch(title))
                .Select(keyValuePair => keyValuePair.Key).ToList();
        
        public static IEnumerable<TechnologiesEnum> GetTechnologies(IEnumerable<string> jobContents) =>
            jobContents.Select(GetTechnologies).SelectMany(e => e);
    }

}