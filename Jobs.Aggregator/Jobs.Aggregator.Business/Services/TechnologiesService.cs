using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jobs.Aggregator.Core.Models;

namespace Jobs.Aggregator.Core.Services
{
    public static class TechnologiesService
    {
        
        private static readonly RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

        
        private static readonly Dictionary<TechnologiesEnum, TechnologyData> TechnosData =
            new Dictionary<TechnologiesEnum, TechnologyData>
            {
                {TechnologiesEnum.Java, new TechnologyData{ Regex = new Regex("java|jee|boot|spring", _options), DisplayName = "Java"}},
                {TechnologiesEnum.Dotnet, new TechnologyData{ Regex = new Regex(@"c#|\.net", _options), DisplayName = ".NET / .NET Core"}},
                {TechnologiesEnum.Nodejs, new TechnologyData{ Regex = new Regex(@"node", _options), DisplayName = "NodeJS"}},
                {TechnologiesEnum.Vuejs, new TechnologyData{ Regex = new Regex(@" vue |vuejs", _options), DisplayName = "Vue"}},
                {TechnologiesEnum.React, new TechnologyData{ Regex = new Regex(@"react", _options), DisplayName = "React"}},
                {TechnologiesEnum.Angular, new TechnologyData{ Regex = new Regex(@"angular", _options), DisplayName = "Angular"}},
                {TechnologiesEnum.Golang, new TechnologyData{ Regex = new Regex(@" go |golang", _options), DisplayName = "Go"}},
                {TechnologiesEnum.Php, new TechnologyData{ Regex = new Regex(@"php", _options), DisplayName = "PHP"}},
                {TechnologiesEnum.Symfony, new TechnologyData{ Regex = new Regex(@"symfony", _options), DisplayName = "Symfony"}},
                {TechnologiesEnum.Laravel, new TechnologyData{ Regex = new Regex(@"laravel", _options), DisplayName = "Laravel"}},
                {TechnologiesEnum.Cpp, new TechnologyData{ Regex = new Regex(@"c\+\+", _options), DisplayName = "C++" }},
                {TechnologiesEnum.C, new TechnologyData{ Regex = new Regex(@" c ", _options), DisplayName = "C" }},
                {TechnologiesEnum.Devops, new TechnologyData{ Regex = new Regex(@"devops", _options), DisplayName = "DevOps" }},
                {TechnologiesEnum.Cloud, new TechnologyData{ Regex = new Regex(@" cloud ", _options), DisplayName = "Cloud" }},
                {TechnologiesEnum.Embedded, new TechnologyData{ Regex = new Regex(@" syst(e|è)mes? embarqués? ", _options), DisplayName = "Systèmes Embarqués" }},
                {TechnologiesEnum.Android, new TechnologyData{ Regex = new Regex(@" android ", _options), DisplayName = "Android" }},
                {TechnologiesEnum.Ios, new TechnologyData{ Regex = new Regex(@" ios |swift", _options), DisplayName = "IOS" }},
                {TechnologiesEnum.Arduino, new TechnologyData{ Regex = new Regex(@"arduino", _options), DisplayName = "Arduino" }},
                {TechnologiesEnum.Tensorflow, new TechnologyData{ Regex = new Regex(@"tensorflow", _options), DisplayName = "Tensorflow" }},
                {TechnologiesEnum.Javascript, new TechnologyData{ Regex = new Regex(@"javascript", _options), DisplayName = "Javascript" }},
                {TechnologiesEnum.Typescript, new TechnologyData{ Regex = new Regex(@"typescript", _options), DisplayName = "Typescript" }},
                {TechnologiesEnum.Python, new TechnologyData{ Regex = new Regex(@"python", _options), DisplayName = "Python" }},
                {TechnologiesEnum.Hardware, new TechnologyData{ Regex = new Regex(@"hardware", _options), DisplayName = "Hardware" }},
                {TechnologiesEnum.Iot, new TechnologyData{ Regex = new Regex(@" iot | internet of things", _options), DisplayName = "IOT" }},
                {TechnologiesEnum.Sql, new TechnologyData{ Regex = new Regex(@"sql", _options), DisplayName = "SQL" }},
                {TechnologiesEnum.PlSql, new TechnologyData{ Regex = new Regex(@"pl ?/ ?sql | pl ?sql", _options), DisplayName = "PL/SQL" }},
                {TechnologiesEnum.TSql, new TechnologyData{ Regex = new Regex(@"t-?sql", _options), DisplayName = "TSQL" }},
                {TechnologiesEnum.Aws, new TechnologyData{ Regex = new Regex(@"aws|amazon web services", _options), DisplayName = "AWS" }},
                {TechnologiesEnum.Gcp, new TechnologyData{ Regex = new Regex(@"gcp|google cloud", _options), DisplayName = "Google Cloud" }},
                {TechnologiesEnum.Azure, new TechnologyData{ Regex = new Regex(@"azure", _options), DisplayName = "Azure" }},
                {TechnologiesEnum.Jenkins, new TechnologyData{ Regex = new Regex(@"jenkins", _options), DisplayName = "Jenkins" }},
                {TechnologiesEnum.Teamcity, new TechnologyData{ Regex = new Regex(@"teamcity", _options), DisplayName = "Teamcity" }},
            };


        public static IEnumerable<TechnologiesEnum> GetTechnologies(string title) =>
            TechnosData.Where(
                    keyValuePair => TechnosData[keyValuePair.Key].Regex.IsMatch(title))
                .Select(keyValuePair => keyValuePair.Key).ToList();

        public static IEnumerable<TechnologiesEnum> GetTechnologies(IEnumerable<string> jobContents) =>
            jobContents.Select(GetTechnologies).SelectMany(e => e);

        public static string GetTechnologyName(TechnologiesEnum enumMember) => TechnosData[enumMember].DisplayName;

        public static IEnumerable<TechnologiesEnum> GetAllTechnologies() => TechnosData.Select(e => e.Key);
    }

}