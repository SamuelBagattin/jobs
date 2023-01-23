namespace Jobs.Aggregator.Core.TransitionModels;

public record Technology
{
    public TechnologiesEnum TechnologyName { get; init; } 
    public int JobsCountWithPrimaryTechnology { get; init; }
}