resource "aws_lambda_function" "aggregator" {
  function_name = local.aggregator_lambda_name
  handler       = "Jobs.Aggregator.Local::Jobs.Aggregator.Local.Program::Main"
  role          = aws_iam_role.aggregator_lambda.arn
  runtime       = "dotnetcore3.1"
  filename      = "Jobs.Aggregator.Local.zip"
  timeout       = 30
  memory_size   = 512

  environment {
    variables = {
      ON_LAMBDA : true
      SOURCE_DATA_BUCKET_NAME : aws_s3_bucket.scraper_results.bucket
      DESTINATION_DATA_BUCKET_NAME : aws_s3_bucket.aggregated_results.bucket
      DESTINATION_DATA_DISTRIBUTION_ID : aws_cloudfront_distribution.s3_distribution.id
    }
  }

  tags = {
    Project : local.project_name
    Name : local.aggregator_lambda_name
  }

}

resource "aws_iam_role" "aggregator_lambda" {
  assume_role_policy = data.aws_iam_policy_document.aggregator_assume_role.json
  name               = local.aggregator_iam_role_name
  tags = {
    Project : local.project_name
    Name : local.aggregator_iam_role_name
  }
}

data "aws_iam_policy_document" "aggregator_assume_role" {
  version = "2012-10-17"
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      identifiers = ["lambda.amazonaws.com"]
      type        = "Service"
    }
    effect = "Allow"
  }
}

resource "aws_iam_policy_attachment" "aggregator_policy_to_role" {
  name       = local.aggregator_policy_attachment_name
  policy_arn = aws_iam_policy.aggregator_policy.arn
  roles      = [aws_iam_role.aggregator_lambda.name]
}

resource "aws_iam_policy" "aggregator_policy" {
  policy = data.aws_iam_policy_document.aggregator_policy.json
  name   = local.aggregator_iam_policy_name
}

data "aws_iam_policy_document" "aggregator_policy" {
  version = "2012-10-17"
  statement {
    actions   = ["s3:GetObject"]
    effect    = "Allow"
    resources = ["${aws_s3_bucket.scraper_results.arn}/*"]
  }
  statement {
    actions   = ["s3:PutObject"]
    effect    = "Allow"
    resources = ["${aws_s3_bucket.aggregated_results.arn}/*"]
  }
  statement {
    actions   = ["s3:ListBucket"]
    effect    = "Allow"
    resources = [aws_s3_bucket.scraper_results.arn]
  }
  statement {
    actions   = ["sqs:ReceiveMessage", "sqs:DeleteMessage", "sqs:GetQueueAttributes"]
    effect    = "Allow"
    resources = [aws_sqs_queue.aggregator_trigger.arn]
  }
  statement {
    actions   = ["cloudfront:CreateInvalidation"]
    effect    = "Allow"
    resources = [aws_cloudfront_distribution.s3_distribution.arn]
  }
}

resource "aws_lambda_event_source_mapping" "aggregator_sqs_trigger" {
  event_source_arn = aws_sqs_queue.aggregator_trigger.arn
  function_name    = aws_lambda_function.aggregator.arn
}







