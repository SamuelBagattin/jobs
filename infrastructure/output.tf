output "scraper_results_bucket_arn" {
  value = aws_s3_bucket.scraper_results.arn

}
output "aggregator_results_bucket_arn" {
  value = aws_s3_bucket.aggregated_results.arn
}

output "scraper_results_bucket_name" {
  value = aws_s3_bucket.scraper_results.bucket
}

output "scraper_trigger_sqs_queue_url" {
  value = aws_sqs_queue.scraper_trigger.url
}

output "scraper_iam_role_arn" {
  value = aws_iam_role.scraper.arn
}

output "post_scraping_sns_topic_arn" {
  value = aws_sns_topic.post_scraping.arn
}
