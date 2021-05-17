////
// Bucket
////
resource "aws_s3_bucket" "scraper_results" {
  bucket = local.scraper_results_bucket_name
  acl    = "private"
  tags = {
    Name    = local.scraper_results_bucket_name
    Project = local.project_name
  }
}


////
// EventBridge
////
resource "aws_cloudwatch_event_rule" "scraper_trigger" {
  name                = local.scraper_event_trigger_name
  schedule_expression = "cron(0 2 * * ? *)"
  tags = {
    Project : local.project_name
    Name : local.scraper_event_trigger_name
  }
}

resource "aws_cloudwatch_event_target" "scraper_target" {
  target_id = "${aws_iam_user.scraper.name}-target"
  arn       = aws_sqs_queue.scraper_trigger.arn
  rule      = aws_cloudwatch_event_rule.scraper_trigger.name
}




////
// IAM
////

resource "aws_iam_role" "scraper" {
  name               = local.scraper_iam_role_name
  assume_role_policy = data.aws_iam_policy_document.scraper_assume_role.json
  tags = {
    Project : local.project_name
    Name : local.scraper_iam_role_name
  }
}

resource "aws_iam_policy_attachment" "scraper_attachment_to_basicrole" {
  name       = "${local.scraper_policy_attachment}_to_basicexecutionrole"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
  roles      = [aws_iam_role.scraper.name]
}

resource "aws_iam_policy_attachment" "scraper_attachment" {
  name       = local.scraper_policy_attachment
  policy_arn = aws_iam_policy.scraper_policy.arn
  roles      = [aws_iam_role.scraper.name]
}

data "aws_iam_policy_document" "scraper_assume_role" {
  version = "2012-10-17"
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      identifiers = [aws_iam_user.scraper.arn, data.aws_caller_identity.identity.arn]
      type        = "AWS"
    }
    effect = "Allow"
    sid    = ""
  }
}

resource "aws_iam_policy" "scraper_policy" {
  name   = local.scraper_iam_policy_name
  policy = data.aws_iam_policy_document.scraper_policy.json
}

data "aws_iam_policy_document" "scraper_policy" {
  version = "2012-10-17"
  statement {
    actions = [
      "s3:GetObject",
      "s3:PutObject",
      "s3:DeleteObject"
    ]
    effect    = "Allow"
    resources = ["${aws_s3_bucket.scraper_results.arn}/*"]
  }
  statement {
    actions   = ["s3:ListBucket"]
    effect    = "Allow"
    resources = [aws_s3_bucket.scraper_results.arn]
  }
  statement {
    actions   = ["ssm:GetParameter"]
    effect    = "Allow"
    resources = [aws_ssm_parameter.scraping_queries.arn]
  }
  statement {
    actions   = ["sqs:ReceiveMessage", "sqs:DeleteMessage", "sqs:GetQueueAttributes"]
    effect    = "Allow"
    resources = [aws_sqs_queue.scraper_trigger.arn]
  }
  statement {
    actions   = ["lambda:InvokeAsync"]
    effect    = "Allow"
    resources = [aws_lambda_function.aggregator.arn]
  }
}


// On Prem scraper
resource "aws_iam_user" "scraper" {
  name = local.scraper_user_name
}
