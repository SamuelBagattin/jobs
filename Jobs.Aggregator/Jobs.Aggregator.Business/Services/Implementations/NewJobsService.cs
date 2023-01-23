using System.Collections.Generic;
using System.Linq;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Core.FinalModels;
using Jobs.Aggregator.Core.Services.Contracts;

namespace Jobs.Aggregator.Core.Services.Implementations;

public class NewJobsService : INewJobsService
{
    public List<FinalCompany> GetNewJobs(ResponseRoot previousData, ResponseRoot newData)
    {
        var previousJobsIds = previousData.Companies.Companies.SelectMany(e => e.Jobs).Select(e => e.Id);
            
        var newJobsCompaniesToReturn = new List<FinalCompany>();
        foreach (var companiesCompany in newData.Companies.Companies)
        {
            var newJobsForCompany = companiesCompany.Jobs.Where(e => !previousJobsIds.Contains(e.Id)).ToList();
                
            if (!newJobsForCompany.Any()) continue;
                
            var companyCopy = new FinalCompany
            {
                Id = companiesCompany.Id,
                Jobs = newJobsForCompany,
                CompanyName = companiesCompany.CompanyName,
                MainTechnologies = companiesCompany.MainTechnologies,
                SecondaryTechnologies = companiesCompany.SecondaryTechnologies
            };
            newJobsCompaniesToReturn.Add(companyCopy);
        }

        return newJobsCompaniesToReturn;
    }
}