using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jobs.Aggregator.Core.Models;
using Jobs.Aggregator.Core.Services.Contracts;

namespace Jobs.Aggregator.Core.Services.Implementations
{
    public class TechnologiesService : ITechnologiesService
    {
        private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Multiline;


        private static readonly Dictionary<TechnologiesEnum, TechnologyData> TechnosData =
            new()
            {
                {
                    TechnologiesEnum.Java,
                    new TechnologyData {Regex = new Regex("java|jee|boot|spring", Options), DisplayName = "Java"}
                },
                {
                    TechnologiesEnum.Dotnet,
                    new TechnologyData {Regex = new Regex(@"c#|\.net", Options), DisplayName = ".NET / .NET Core"}
                },
                {
                    TechnologiesEnum.Nodejs,
                    new TechnologyData {Regex = new Regex(@"node", Options), DisplayName = "NodeJS"}
                },
                {
                    TechnologiesEnum.Vuejs,
                    new TechnologyData {Regex = new Regex(@" vue |vuejs", Options), DisplayName = "Vue"}
                },
                {
                    TechnologiesEnum.React,
                    new TechnologyData {Regex = new Regex(@"react", Options), DisplayName = "React"}
                },
                {
                    TechnologiesEnum.Angular,
                    new TechnologyData {Regex = new Regex(@"angular", Options), DisplayName = "Angular"}
                },
                {
                    TechnologiesEnum.Golang,
                    new TechnologyData {Regex = new Regex(@" go |golang", Options), DisplayName = "Go"}
                },
                {TechnologiesEnum.Php, new TechnologyData {Regex = new Regex(@"php", Options), DisplayName = "PHP"}},
                {
                    TechnologiesEnum.Symfony,
                    new TechnologyData {Regex = new Regex(@"symfony", Options), DisplayName = "Symfony"}
                },
                {
                    TechnologiesEnum.Laravel,
                    new TechnologyData {Regex = new Regex(@"laravel", Options), DisplayName = "Laravel"}
                },
                {TechnologiesEnum.Cpp, new TechnologyData {Regex = new Regex(@"c\+\+", Options), DisplayName = "C++"}},
                {TechnologiesEnum.C, new TechnologyData {Regex = new Regex(@" c ", Options), DisplayName = "C"}},
                {
                    TechnologiesEnum.Devops,
                    new TechnologyData {Regex = new Regex(@"devops", Options), DisplayName = "DevOps"}
                },
                {
                    TechnologiesEnum.Cloud,
                    new TechnologyData {Regex = new Regex(@" cloud ", Options), DisplayName = "Cloud"}
                },
                {
                    TechnologiesEnum.Embedded,
                    new TechnologyData
                        {Regex = new Regex(@" syst(e|è)mes? embarqués? ", Options), DisplayName = "Systèmes Embarqués"}
                },
                {
                    TechnologiesEnum.Android,
                    new TechnologyData {Regex = new Regex(@" android ", Options), DisplayName = "Android"}
                },
                {
                    TechnologiesEnum.Ios,
                    new TechnologyData {Regex = new Regex(@" ios |swift", Options), DisplayName = "IOS"}
                },
                {
                    TechnologiesEnum.Arduino,
                    new TechnologyData {Regex = new Regex(@"arduino", Options), DisplayName = "Arduino"}
                },
                {
                    TechnologiesEnum.Tensorflow,
                    new TechnologyData {Regex = new Regex(@"tensorflow", Options), DisplayName = "Tensorflow"}
                },
                {
                    TechnologiesEnum.Javascript,
                    new TechnologyData {Regex = new Regex(@"javascript", Options), DisplayName = "Javascript"}
                },
                {
                    TechnologiesEnum.Typescript,
                    new TechnologyData {Regex = new Regex(@"typescript", Options), DisplayName = "Typescript"}
                },
                {
                    TechnologiesEnum.Python,
                    new TechnologyData {Regex = new Regex(@"python", Options), DisplayName = "Python"}
                },
                {
                    TechnologiesEnum.Hardware,
                    new TechnologyData {Regex = new Regex(@"hardware", Options), DisplayName = "Hardware"}
                },
                {
                    TechnologiesEnum.Iot,
                    new TechnologyData {Regex = new Regex(@" iot | internet of things", Options), DisplayName = "IOT"}
                },
                {TechnologiesEnum.Sql, new TechnologyData {Regex = new Regex(@"sql", Options), DisplayName = "SQL"}},
                {
                    TechnologiesEnum.PlSql,
                    new TechnologyData {Regex = new Regex(@"pl ?/ ?sql | pl ?sql", Options), DisplayName = "PL/SQL"}
                },
                {
                    TechnologiesEnum.TSql,
                    new TechnologyData {Regex = new Regex(@"t-?sql", Options), DisplayName = "TSQL"}
                },
                {
                    TechnologiesEnum.Aws,
                    new TechnologyData {Regex = new Regex(@"aws|amazon web services", Options), DisplayName = "AWS"}
                },
                {
                    TechnologiesEnum.Gcp,
                    new TechnologyData {Regex = new Regex(@"gcp|google cloud", Options), DisplayName = "Google Cloud"}
                },
                {
                    TechnologiesEnum.Azure,
                    new TechnologyData {Regex = new Regex(@"azure", Options), DisplayName = "Azure"}
                },
                {
                    TechnologiesEnum.Jenkins,
                    new TechnologyData {Regex = new Regex(@"jenkins", Options), DisplayName = "Jenkins"}
                },
                {
                    TechnologiesEnum.Teamcity,
                    new TechnologyData {Regex = new Regex(@"teamcity", Options), DisplayName = "Teamcity"}
                }
            };


        public IEnumerable<TechnologiesEnum> GetTechnologies(string text)
        {
            return TechnosData.Where(
                    keyValuePair => TechnosData[keyValuePair.Key].Regex.IsMatch(text))
                .Select(keyValuePair => keyValuePair.Key).ToList();
        }

        public IEnumerable<TechnologiesEnum> GetTechnologies(IEnumerable<string> jobContents)
        {
            return jobContents.Select(GetTechnologies).SelectMany(e => e);
        }

        public string GetTechnologyName(TechnologiesEnum enumMember)
        {
            return TechnosData[enumMember].DisplayName;
        }

        public IEnumerable<TechnologiesEnum> GetAllTechnologies()
        {
            return TechnosData.Select(e => e.Key);
        }
    }
}