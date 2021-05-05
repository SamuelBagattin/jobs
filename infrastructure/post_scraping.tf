resource "aws_sns_topic" "post_scraping" {
  name                                  = local.scraping_destination_sns_topic_name
  sqs_failure_feedback_role_arn         = aws_iam_role.sns_topic_feedback.arn
  sqs_success_feedback_role_arn         = aws_iam_role.sns_topic_feedback.arn
  application_failure_feedback_role_arn = aws_iam_role.sns_topic_feedback.arn
  application_success_feedback_role_arn = aws_iam_role.sns_topic_feedback.arn
  tags = {
    Project : local.project_name
    Name : local.scraping_destination_sns_topic_name
  }
}

resource "aws_sns_topic_subscription" "post_scraping_to_aggregator_trigger" {
  endpoint  = aws_sqs_queue.aggregator_trigger.arn
  protocol  = "sqs"
  topic_arn = aws_sns_topic.post_scraping.arn
}

data "aws_iam_policy_document" "sns_assumerole" {
  version = "2012-10-17"
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      identifiers = ["sns.amazonaws.com"]
      type        = "Service"
    }
    effect = "Allow"
    sid    = ""
  }
}

resource "aws_sqs_queue" "aggregator_trigger" {
  name                       = local.aggregator_trigger_sqs_queue_name
  visibility_timeout_seconds = 90
  tags = {
    Project : local.project_name
    Name : local.aggregator_trigger_sqs_queue_name
  }

}

resource "aws_sqs_queue" "scraping_publisher_trigger" {
  name                       = local.scraping_publisher_sqs_queue_name
  visibility_timeout_seconds = 5
  tags = {
    Project : local.project_name
    Name : local.scraping_publisher_sqs_queue_name
  }

}

resource "aws_sqs_queue_policy" "aggregator_trigger" {
  policy    = <<POLICY
{
  "Version": "2012-10-17",
  "Id": "sqspolicy",
  "Statement": [
    {
      "Sid": "First",
      "Effect": "Allow",
      "Principal": {
        "Service": "sns.amazonaws.com"
      },
      "Action": "sqs:SendMessage",
      "Resource": "${aws_sqs_queue.aggregator_trigger.arn}",
      "Condition": {
        "ArnEquals": {
          "aws:SourceArn": "${aws_sns_topic.post_scraping.arn}"
        }
      }
    }
  ]
}
POLICY
  queue_url = aws_sqs_queue.aggregator_trigger.id
}

////
// IAM
////
resource "aws_iam_policy" "allow_publish_message_to_scraper_snstopic" {
  name   = local.scraper_sns_topic_policy_name
  policy = data.aws_iam_policy_document.allow_publish_message_to_scraper_snstopic.json
}

data "aws_iam_policy_document" "allow_publish_message_to_scraper_snstopic" {
  statement {
    actions = [
      "sns:Publish"
    ]
    effect    = "Allow"
    resources = [aws_sns_topic.post_scraping.arn]
  }
}

resource "aws_iam_policy_attachment" "lambda_scraper_sns_topic" {
  policy_arn = aws_iam_policy.allow_publish_message_to_scraper_snstopic.arn
  name       = "${aws_iam_policy.allow_publish_message_to_scraper_snstopic.name}_${aws_iam_role.scraper.name}_attachment"
  roles      = [aws_iam_role.scraper.name]
}


resource "aws_iam_role" "sns_topic_feedback" {
  assume_role_policy = data.aws_iam_policy_document.sns_assumerole.json
  name               = "${local.scraping_destination_sns_topic_name}-cloudwatchlgs-access"
}

resource "aws_iam_policy_attachment" "sns_topic_failure_feedback_cloudwatchlogs_access_attachment" {
  name       = "${aws_iam_role.sns_topic_feedback.name}-${aws_iam_policy.cloudwatchlogs_access.name}-policy-attachment"
  policy_arn = aws_iam_policy.cloudwatchlogs_access.arn
  roles      = [aws_iam_role.sns_topic_feedback.name]
}
