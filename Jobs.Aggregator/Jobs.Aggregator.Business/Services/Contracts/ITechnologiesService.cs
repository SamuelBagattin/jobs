using System.Collections.Generic;

namespace Jobs.Aggregator.Core.Services.Contracts
{
    public interface ITechnologiesService
    {
        IEnumerable<TechnologiesEnum> GetTechnologies(string title);
        IEnumerable<TechnologiesEnum> GetTechnologies(IEnumerable<string> jobContents);
        string GetTechnologyName(TechnologiesEnum enumMember);
        IEnumerable<TechnologiesEnum> GetAllTechnologies();
    }
}