namespace Jobs.Aggregator.Core.Services.Contracts
{
    public interface IIdService
    {
        ulong GetJobId(string companyName, string jobName);
    }
}