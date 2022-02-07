using System.Collections.Generic;
using Jobs.Aggregator.Core.FinalModels;

namespace Jobs.Aggregator.Core.Services.Contracts
{
    public interface INewJobsService
    {
        List<FinalCompany> GetNewJobs(ResponseRoot previousData, ResponseRoot newData);
    }
}