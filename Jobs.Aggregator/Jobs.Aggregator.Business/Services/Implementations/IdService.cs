using Jobs.Aggregator.Core.Services.Contracts;

namespace Jobs.Aggregator.Core.Services.Implementations
{
    public class IdService : IIdService
    {
        public ulong GetJobId(string companyName, string jobName)
        {
            return CalculateHash(companyName + ":" + jobName);
        }
        
        private static ulong CalculateHash(string read)
        {
            var hashedValue = 3074457345618258791ul;
            foreach (var t in read)
            {
                hashedValue += t;
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }
    }
}