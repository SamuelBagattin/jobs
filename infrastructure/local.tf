locals {
  website_domain = "jobs.samuelbagattin.com"
  base_domain    = "samuelbagattin.com"
  project_name   = "jobs"

  scraper_results_bucket_name         = "${local.project_name}-scraper-results"
  scraper_lambda_name                 = "${local.project_name}-scraper-lambda"
  scraper_iam_role_name               = "${local.project_name}-scraper-role"
  scraper_iam_policy_name             = "${local.project_name}-scraper-policy"
  scraper_policy_attachment           = "${local.project_name}-scraper-policy-attachment"
  scraper_sns_topic_policy_name       = "${local.project_name}-scraper-allow-publishsnstopic-policy"
  scraping_destination_sns_topic_name = "${local.project_name}-scraping-destination-snstopic"
  scraper_event_trigger_name          = "${local.project_name}-scraper-trigger-eventbridget-rule"

  aggregator_trigger_sqs_queue_name = "${local.project_name}-aggregator-trigger-sqs-queue"
  aggregator_results_bucket_name    = "${local.project_name}-aggregator-results"
  aggregator_lambda_name            = "${local.project_name}-aggregator-lambda"
  aggregator_iam_role_name          = "${local.project_name}-aggregator-role"
  aggregator_iam_policy_name        = "${local.project_name}-aggregator-policy"
  aggregator_policy_attachment_name = "${local.project_name}-aggregator-policy-attachment"

  website_distribution_name = "${local.project_name}-website-distribution"
  website_origin_id         = "${local.project_name}-website-origin"
}
