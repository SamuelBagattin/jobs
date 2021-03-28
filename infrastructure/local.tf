locals {
  website_domain = "jobs.samuelbagattin.com"
  base_domain = "samuelbagattin.com"
  project_name = "jobs"
  scraper_results_bucket_name = "${local.project_name}-scraper-results"
  aggregator_results_bucket_name = "${local.project_name}-aggregator-results"
  scraper_lambda_name = "${local.project_name}-scrape-lambda"
  scraper_iam_role_name = "${local.project_name}-scraper-role"
  scraper_iam_policy_name = "${local.project_name}-scraper-policy"
  scraper_policy_attachment = "${local.project_name}-scraper-policy-attachment"
  scraper_event_trigger_name = "${local.project_name}-scraper-trigger-eventbridget-rule"
  website_distribution_name = "${local.project_name}-website-distribution"
  website_origin_id = "${local.project_name}-website-origin"
}
