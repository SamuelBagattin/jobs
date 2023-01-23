using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws.Models;

namespace Jobs.Aggregator.Aws.Services.Contracts
{
    public interface IScraperResultsService
    {
        Task<IEnumerable<Job>> GetAllScrapedJobs(CancellationToken cancellationToken);
    }
}