resource "aws_sns_topic" "scraper_event_bus" {
  name = local.scraping_destination_sns_topic_name
  sqs_failure_feedback_role_arn = aws_iam_role.sns_topic_failure_feedback.arn
  sqs_success_feedback_role_arn = aws_iam_role.sns_topic_failure_feedback.arn
  application_failure_feedback_role_arn = aws_iam_role.sns_topic_failure_feedback.arn
  application_success_feedback_role_arn = aws_iam_role.sns_topic_failure_feedback.arn
  tags = {
    Project: local.project_name
    Name: local.scraping_destination_sns_topic_name
  }
}


data "aws_iam_policy_document" "sns_assumerole" {
  version = "2012-10-17"
  statement {
    actions = [
      "sts:AssumeRole"]
    principals {
      identifiers = [
        "sns.amazonaws.com"]
      type = "Service"
    }
    effect = "Allow"
    sid = ""
  }
}


resource "aws_lambda_function_event_invoke_config" "scraper_to_eventbus" {
  function_name = aws_lambda_function.scraper.function_name
  destination_config {
    on_success {
      destination = aws_sns_topic.scraper_event_bus.arn
    }
  }
  depends_on = [
    aws_iam_policy_attachment.lambda_scraper_sns_topic]
}


resource "aws_sqs_queue" "aggregator_trigger" {
  name = local.aggregator_trigger_sqs_queue_name
  tags = {
    Project: local.project_name
    Name: local.aggregator_trigger_sqs_queue_name
  }

}

resource "aws_sqs_queue_policy" "aggregator_trigger" {
  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Id": "sqspolicy",
  "Statement": [
    {
      "Sid": "First",
      "Effect": "Allow",
      "Principal": "*",
      "Action": "sqs:SendMessage",
      "Resource": "${aws_sqs_queue.aggregator_trigger.arn}",
      "Condition": {
        "ArnEquals": {
          "aws:SourceArn": "${aws_sns_topic.scraper_event_bus.arn}"
        }
      }
    }
  ]
}
POLICY
  queue_url =  aws_sqs_queue.aggregator_trigger.id
}

resource "aws_sns_topic_subscription" "scraper-target-to-aggregator-trigger" {
  topic_arn = aws_sns_topic.scraper_event_bus.arn
  protocol = "sqs"
  endpoint = aws_sqs_queue.aggregator_trigger.arn

}


////
// IAM
////
resource "aws_iam_policy" "allow_publish_message_to_scraper_snstopic" {
  name = local.scraper_sns_topic_policy_name
  policy = data.aws_iam_policy_document.allow_publish_message_to_scraper_snstopic.json
}

data "aws_iam_policy_document" "allow_publish_message_to_scraper_snstopic" {
  statement {
    actions = [
      "sns:Publish"
    ]
    effect = "Allow"
    resources = [
      aws_sns_topic.scraper_event_bus.arn]
  }
}

resource "aws_iam_policy_attachment" "lambda_scraper_sns_topic" {
  policy_arn = aws_iam_policy.allow_publish_message_to_scraper_snstopic.arn
  name = "${aws_iam_policy.allow_publish_message_to_scraper_snstopic.name}_${aws_iam_role.lambda_scraper.name}_attachment"
  roles = [
    aws_iam_role.lambda_scraper.name]
}


resource "aws_iam_role" "sns_topic_failure_feedback" {
  assume_role_policy = data.aws_iam_policy_document.sns_assumerole.json
  name = "${local.scraping_destination_sns_topic_name}-cloudwatchlogs-access"
}

resource "aws_iam_policy_attachment" "sns_topic_failure_feedback_cloudwatchlogs_access_attachment" {
  name = "${aws_iam_role.sns_topic_failure_feedback.name}-${aws_iam_policy.cloudwatchlogs_access.name}-policy-attachment"
  policy_arn = aws_iam_policy.cloudwatchlogs_access.arn
  roles = [
    aws_iam_role.sns_topic_failure_feedback.name]
}

resource "aws_iam_policy" "cloudwatchlogs_access" {
  name = "${local.project_name}-cloudwatchlogs-access-policy"
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
    resources = [
      "*"]
  }
}
