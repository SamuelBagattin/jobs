using System;

namespace Jobs.Aggregator.Aws.Models
{
    public record Job
    {
        public string Id { get; set; }
        public string Title { get; init; }
        public string Company { get; init; }
        public string Site { get; init; }
        public string Url { get; init; }
        public DateTime? Scrapedate { get; set; }
        public string Description { get; init; }
    }
}