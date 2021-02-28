using System;

namespace Jobs.Aggregator.Aws
{
    public class Job
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Site { get; set; }
        public string Url { get; set; }
        public DateTime? Scrapedate { get; set; }
        public string Description { get; set; }
    }
}