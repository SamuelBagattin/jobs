output "s3_bucket_scraper_results" {
  value = {
    arn = aws_s3_bucket.scraper_results.arn
  }
}
output "s3_bucket_aggregator_results" {
  value = {
    arn = aws_s3_bucket.aggregated_results.arn
  }
}
