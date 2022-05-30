resource "aws_sqs_queue" "scraper_trigger" {
  name                       = local.scraper_trigger_sqs_queue_name
  visibility_timeout_seconds = 12 * 60 * 60
  receive_wait_time_seconds  = 20

  tags = {
    Project : local.project_name
    Name : local.scraper_trigger_sqs_queue_name
  }

}

resource "aws_sqs_queue_policy" "scraper_trigger" {
  policy    = <<POLICY
{
  "Version": "2012-10-17",
  "Id": "sqspolicy",
  "Statement": [
    {
      "Sid": "First",
      "Effect": "Allow",
      "Principal": {
        "Service": "events.amazonaws.com"
      },
      "Action": "sqs:SendMessage",
      "Resource": "${aws_sqs_queue.scraper_trigger.arn}",
      "Condition": {
        "ArnEquals": {
          "aws:SourceArn": "${aws_cloudwatch_event_rule.scraper_trigger.arn}"
        }
      }
    }
  ]
}
POLICY
  queue_url = aws_sqs_queue.scraper_trigger.id
}

resource "aws_iam_policy" "cloudwatchlogs_access" {
  name   = "${local.project_name}-cloudwatchlogs-access-policy"
  policy = data.aws_iam_policy_document.cloudwatchlogs_access.json
}

data "aws_iam_policy_document" "cloudwatchlogs_access" {
  statement {
    effect = "Allow"
    actions = [
      "logs:CreateLogGroup",
      "logs:CreateLogStream",
      "logs:PutLogEvents",
      "logs:PutMetricFilter",
      "logs:PutRetentionPolicy"
    ]
    resources = ["*"]
  }
}
