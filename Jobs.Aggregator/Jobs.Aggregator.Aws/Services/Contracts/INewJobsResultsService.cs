using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jobs.Aggregator.Core.FinalModels;

namespace Jobs.Aggregator.Aws.Services.Contracts
{
    public interface INewJobsResultsService
    {
        Task UploadNewJobs(List<FinalCompany> newJobs, CancellationToken cancellationToken);
    }
}