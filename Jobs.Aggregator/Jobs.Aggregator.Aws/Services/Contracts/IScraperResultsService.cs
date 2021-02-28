using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jobs.Aggregator.Aws.Services.Contracts
{
    public interface IScraperResultsService
    {
        Task<IEnumerable<Job>> GetAllScrapedJobs();
    }
}