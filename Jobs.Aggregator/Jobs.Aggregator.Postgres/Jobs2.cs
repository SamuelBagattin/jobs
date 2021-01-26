namespace Jobs.Aggregator.Postgres
{
    public partial class Jobs
    {
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Title)}: {Title}, {nameof(Company)}: {Company}, {nameof(Site)}: {Site}, {nameof(Url)}: {Url}, {nameof(Scrapedate)}: {Scrapedate}";
        }
    }
}