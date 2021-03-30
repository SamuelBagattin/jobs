////
// Lambda
////
resource "aws_lambda_function" "scraper" {
  function_name = local.scraper_lambda_name
  handler       = "main"
  role          = aws_iam_role.lambda_scraper.arn
  runtime       = "go1.x"
  filename      = "main.zip"
  timeout       = 900

  environment {
    variables = {
      ON_LAMBDA : true
      RESULTS_BUCKET_NAME : aws_s3_bucket.scraper_results.bucket
    }
  }

  tags = {
    Project : local.project_name
    Name : local.scraper_lambda_name
  }

}


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
  schedule_expression = "cron(0 10 * * ? *)"
  tags = {
    Project : local.project_name
    Name : local.scraper_event_trigger_name
  }
}

resource "aws_cloudwatch_event_target" "scraper_target" {
  target_id = "${aws_lambda_function.scraper.function_name}-target"
  arn       = aws_lambda_function.scraper.arn
  rule      = aws_cloudwatch_event_rule.scraper_trigger.name

}

resource "aws_lambda_permission" "eventbridge_to_scraper_lambda" {
  function_name = aws_lambda_function.scraper.function_name
  statement_id  = "AllowExecutionFromCloudWatch"
  action        = "lambda:InvokeFunction"
  principal     = "events.amazonaws.com"
  source_arn    = aws_cloudwatch_event_rule.scraper_trigger.arn

}


////
// IAM
////

resource "aws_iam_role" "lambda_scraper" {
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
  roles = [
  aws_iam_role.lambda_scraper.name]
}

resource "aws_iam_policy_attachment" "scraper_attachment" {
  name       = local.scraper_policy_attachment
  policy_arn = aws_iam_policy.scraper_policy.arn
  roles = [
  aws_iam_role.lambda_scraper.name]
}

data "aws_iam_policy_document" "scraper_assume_role" {
  version = "2012-10-17"
  statement {
    actions = [
    "sts:AssumeRole"]
    principals {
      identifiers = [
      "lambda.amazonaws.com"]
      type = "Service"
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
    "s3:DeleteObject"]
    effect = "Allow"
    resources = [
    "${aws_s3_bucket.scraper_results.arn}/*"]
  }
  statement {
    actions = [
    "s3:ListBucket"]
    effect = "Allow"
    resources = [
    aws_s3_bucket.scraper_results.arn]
  }
}
