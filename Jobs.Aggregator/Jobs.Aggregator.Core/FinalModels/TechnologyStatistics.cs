namespace Jobs.Aggregator.Core.FinalModels
{
    public class TechnologyStatistics
    {
        public string TechnologyName { get; set; }
        public TechnologyStatisticsJob JobsWithMainTechnology { get; set; }
        public TechnologyStatisticsJob JobsWithSecondaryTechnology { get; set; }
        public TechnologyStatisticsCompany CompaniesWithSecondaryTechnologies { get; set; }
        public TechnologyStatisticsCompany CompaniesWithMainTechnologies { get; set; }
    }
}