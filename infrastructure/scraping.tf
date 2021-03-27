resource "aws_lambda_function" "scraper" {
  function_name = local.scraper_lambda_name
  handler = "main"
  role = aws_iam_role.lambda_scraper.arn
  runtime = "go1.x"
  filename = "main.zip"
  timeout = 900
  environment {
    variables = {
      ON_LAMBDA: true
    }
  }

  provider = "aws.paris"
}

resource "aws_iam_role" "lambda_scraper" {
  name = local.scraper_iam_role_name
  assume_role_policy = data.aws_iam_policy_document.scraper_assume_role.json
}

resource "aws_iam_policy_attachment" "scraper_attachment" {
  name = local.scraper_policy_attachment
  policy_arn = aws_iam_policy.scraper_policy.arn
  roles = [aws_iam_role.lambda_scraper.name]
}

data "aws_iam_policy_document" "scraper_assume_role"{
  version = "2012-10-17"
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      identifiers = ["lambda.amazonaws.com"]
      type = "Service"
    }
    effect = "Allow"
    sid = ""
  }
}

resource "aws_iam_policy" "scraper_policy" {
  name = local.scraper_iam_policy_name
  policy = data.aws_iam_policy_document.scraper_policy.json
}

data "aws_iam_policy_document" "scraper_policy"{
  version = "2012-10-17"
  statement {
    actions = ["s3:GetObject", "s3:PutObject", "s3:*"]
    effect = "Allow"
    resources = ["${aws_s3_bucket.scraper_results.arn}/*"]
  }
  statement {
    actions = ["s3:*"]
    effect = "Allow"
    resources = [aws_s3_bucket.scraper_results.arn]
  }
}
