output "scraper_results_bucket_arn" {
  value       = aws_s3_bucket.scraper_results.arn
  description = "The ARN of the S3 bucket that stores the scraper results"

}
output "aggregator_results_bucket_arn" {
  value       = aws_s3_bucket.aggregated_results.arn
  description = "The ARN of the S3 bucket that stores the aggregated results"
}

output "scraper_results_bucket_name" {
  value       = aws_s3_bucket.scraper_results.bucket
  description = "The name of the S3 bucket that stores the scraper results"
}

output "scraper_trigger_sqs_queue_url" {
  value       = aws_sqs_queue.scraper_trigger.url
  description = "The URL of the SQS queue that triggers the scraper"
}

output "scraper_iam_role_arn" {
  value       = aws_iam_role.scraper.arn
  description = "The ARN of the IAM role that the scraper uses"
}

output "post_scraping_sns_topic_arn" {
  value       = aws_sns_topic.post_scraping.arn
  description = "The ARN of the SNS topic that the scraper uses to post results"
}
