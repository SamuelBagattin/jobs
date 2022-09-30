locals {
  website_domain = "jobs.samuelbagattin.com"
  base_domain    = "samuelbagattin.com"
  project_name   = "jobs"

  lambda_deployments_bucket_name = "${local.project_name}-lambda-deployments"

  scraper_results_bucket_name         = "${local.project_name}-scraper-results"
  scraper_user_name                   = "${local.project_name}-scraper-user"
  scraper_iam_role_name               = "${local.project_name}-scraper-role"
  scraper_iam_policy_name             = "${local.project_name}-scraper-policy"
  scraper_policy_attachment           = "${local.project_name}-scraper-policy-attachment"
  scraper_sns_topic_policy_name       = "${local.project_name}-scraper-allow-publishsnstopic-policy"
  scraping_destination_sns_topic_name = "${local.project_name}-scraping-destination-snstopic"
  scraper_event_trigger_name          = "${local.project_name}-scraper-trigger-eventbridget-rule"
  scraper_trigger_sqs_queue_name      = "${local.project_name}-scraper-trigger-sqs-queue"

  scraping_publisher_sqs_queue_name     = "${local.project_name}-scraping-publisher-sqs-queue"
  scraping_publisher_sqs_dlq_queue_name = "${local.project_name}-scraping-publisher-sqs-dlq"

  aggregator_results_bucket_name        = "${local.project_name}-aggregator-results"
  aggregator_lambda_name                = "${local.project_name}-aggregator-lambda"
  aggregator_iam_role_name              = "${local.project_name}-aggregator-role"
  aggregator_iam_policy_name            = "${local.project_name}-aggregator-policy"
  aggregator_policy_attachment_name     = "${local.project_name}-aggregator-policy-attachment"
  aggregator_trigger_sqs_queue_name     = "${local.project_name}-aggregator-trigger-sqs-queue"
  aggregator_trigger_dlq_sqs_queue_name = "${local.project_name}-aggregator-trigger-sqs-dlq"

  newjobs_history_bucket_name = "${local.project_name}-newjobs-history"

  discord_bot_lambda_name                = "${local.project_name}-discordbot-lambda"
  discord_bot_role_name                  = "${local.project_name}-discordbot-role"
  discord_bot_token_secret_name          = "/${local.project_name}/discordbot/token"
  discord_bot_paste_ee_token_secret_name = "/${local.project_name}/discordbot/pasteee_token"
  discord_bot_iam_policy_name            = "${local.project_name}-discordbot-policy"
  discord_bot_policy_attachment_name     = "${local.project_name}-discordbot-policy-attachment"

  website_distribution_name = "${local.project_name}-website-distribution"
  website_origin_id         = "${local.project_name}-website-origin"

  #  querier_lambda_name                   = "${local.project_name}-querier-lambda"
  #  querier_lambda_role_name              = "${local.project_name}-querier-lambda-role"
  #  querier_api_gateway_name              = "${local.project_name}-querier-apigateway"
  #  querier_lambda_policy_attachment_name = "${local.project_name}-querierlambda-policyattachment"
  #  querier_iam_policy_name               = "${local.project_name}-querierlambda-policy"

  githubactions_jobs_iam_role_name = "githubActions-jobs-role"
}
