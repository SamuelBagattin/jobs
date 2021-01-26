resource "aws_cloudwatch_event_rule" "console" {
  name        = "jobs-scrapper"
  description = "Triggers jobs scrapper every wednesday at midnight"
  schedule_expression = "cron(0 0 ? * WED *)"
}

resource "aws_cloudwatch_event_target" "sns" {
  rule      = aws_cloudwatch_event_rule.console.name
  arn       = module.jobs_scraper.this_lambda_function_arn
}
