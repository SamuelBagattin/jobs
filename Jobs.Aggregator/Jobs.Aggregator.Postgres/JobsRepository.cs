using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Jobs.Aggregator.Postgres
{
    public class JobsRepository
    {
        private readonly JobsContext _context;

        public JobsRepository(JobsContext context)
        {
            _context = context;
        }

        public ImmutableHashSet<Jobs> GetAllJobs()
        {
            Console.WriteLine("Retrieving all jobs from postgres");
            return _context.Jobs.ToImmutableHashSet();
        }
    }
}