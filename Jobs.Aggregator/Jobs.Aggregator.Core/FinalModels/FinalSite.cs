namespace Jobs.Aggregator.Core.FinalModels
{
    public record FinalSite
    {
        public string SiteName { get; init; }
        public string JobUrl { get; set; }
    }
}