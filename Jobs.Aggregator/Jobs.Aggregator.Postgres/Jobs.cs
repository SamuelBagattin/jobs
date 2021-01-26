using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Jobs.Aggregator.Postgres
{
    public partial class Jobs
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Site { get; set; }
        public string Url { get; set; }
        public DateTime? Scrapedate { get; set; }
    }
}
