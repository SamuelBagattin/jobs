resource "aws_s3_bucket" "scraper_results" {
  bucket = "jobs-scraper-results"
  acl = "private"
  tags = {
    Name = "scraper-results"
    Project = "Jobs Scraper"
  }
}
resource "aws_s3_bucket" "aggregated_results" {
  bucket = "jobs-aggregator-results"
  acl = "private"
  tags = {
    Name = "jobs-aggregator-results"
    Project = "Jobs Scraper"
  }
}
