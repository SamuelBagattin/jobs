using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jobs.Aggregator.Core.FinalModels;
using Jobs.Aggregator.Core.Services.Contracts;
using Jobs.Aggregator.Core.TransitionModels;

namespace Jobs.Aggregator.Core.Services.Implementations
{
    public class TechnologiesAggregatorService : ITechnologiesAggregatorService
    {
        private readonly ITechnologiesService _technologiesService;

        public TechnologiesAggregatorService(ITechnologiesService technologiesService)
        {
            _technologiesService = technologiesService;
        }

        public IEnumerable<TechnologyStatistics> GetTechnologiesStatistics(ICollection<AggregatedCompany> res)
        {
            return _technologiesService.GetAllTechnologies().Select(techno => new TechnologyStatistics
            {
                TechnologyName = _technologiesService.GetTechnologyName(techno),
                CompaniesWithMainTechnologies = new TechnologyStatisticsCompany
                {
                    Ids = res.Where(cmp => cmp.MainTechnologies.Contains(techno)).Select(cmp => cmp.Id)
                },
                CompaniesWithSecondaryTechnologies = new TechnologyStatisticsCompany
                {
                    Ids = res.Where(cmp => cmp.SecondaryTechnologies.Contains(techno)).Select(cmp => cmp.Id)
                },
                JobsWithMainTechnology = new TechnologyStatisticsJob
                {
                    Ids = res.SelectMany(cmp => cmp.Jobs)
                        .Where(job => job.Value.MainTechnologies.Contains(techno))
                        .Select(job => job.Value.Id)
                },
                JobsWithSecondaryTechnology = new TechnologyStatisticsJob
                {
                    Ids = res.SelectMany(cmp => cmp.Jobs)
                        .Where(job => job.Value.SecondaryTechnologies.Contains(techno))
                        .Select(job => job.Value.Id)
                }
            });
        }
    }
}