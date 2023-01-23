namespace Jobs.Aggregator.Core.FinalModels
{
    public record Technology
    {
        public string TechnologyName { get; set; }

        public int JobsNumberWithPrimaryTechnology { get; set; }
        
        public int JobsNumberWithSecondaryTechnology { get; set; }
    }
}