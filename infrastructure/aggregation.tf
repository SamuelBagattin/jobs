resource "aws_lambda_function" "scraper" {
  function_name = local.aggregator_lambda_name
  handler = "Jobs.Aggregator.Local::Jobs.Aggregator.Local.Program::Main"
  role = aws_iam_role.aggregator_lambda.arn
  runtime = "dotnetcore3.1"
  filename = "main.zip"
  timeout = 20

  environment {
    variables = {
      ON_LAMBDA: true
      RESULTS_BUCKET_NAME: aws_s3_bucket.scraper_results.bucket
    }
  }

  tags = {
    Project: local.project_name
    Name: local.scraper_iam_role_name
  }

}

resource "aws_iam_role" "aggregator_lambda" {
  assume_role_policy = data.aws_iam_policy_document.aggregator_assume_role
  name = local.aggregator_iam_role_name
}

data "aws_iam_policy_document" "aggregator_assume_role" {
  version = "2012-10-17"
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      identifiers = ["lambda.amazonaws.com"]
      type = "Service"
    }
    effect = "Allow"
  }
}

resource "aws_iam_policy_attachment" "aggregator_policy_to_role" {
  name = local.aggregator_policy_attachment_name
  policy_arn = aws_iam_policy.aggregator_policy.name
  roles = [aws_iam_role.aggregator_lambda]
}

resource "aws_iam_policy" "aggregator_policy" {
  policy = data.aws_iam_policy_document.aggregator_policy
}

data "aws_iam_policy_document" "aggregator_policy" {
  version = "2012-10-17"
  statement {
    actions = ["s3:GetObject"]
    effect = "Allow"
    resources = ["${aws_s3_bucket.scraper_results.arn}/*"]
  }
  statement {
    actions = ["s3:PutObject"]
    effect = "Allow"
    resources = ["${aws_s3_bucket.aggregated_results.arn}/*"]
  }
}

