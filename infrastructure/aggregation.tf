data "archive_file" "aggregator" {
  type = "zip"

  source_dir  = "${path.root}/../out/aggregator"
  output_path = "${path.root}/../out/zip/aggregator/aggregator.zip"
}

resource "aws_s3_object" "aggregator" {
  bucket = module.lambda_deployments_s3_bucket.bucket_name

  key    = "aggregator.zip"
  source = data.archive_file.aggregator.output_path

  etag = filemd5(data.archive_file.aggregator.output_path)
}

resource "aws_lambda_function" "aggregator" {
  function_name    = local.aggregator_lambda_name
  handler          = "Jobs.Aggregator.Local::Jobs.Aggregator.Local.Program::Main"
  role             = aws_iam_role.aggregator_lambda.arn
  runtime          = "dotnet6"
  architectures    = ["arm64"]
  s3_bucket        = aws_s3_object.aggregator.bucket
  s3_key           = aws_s3_object.aggregator.key
  source_code_hash = data.archive_file.aggregator.output_base64sha256
  timeout          = 900
  memory_size      = 4096

  environment {
    variables = {
      ON_LAMBDA : true
      SOURCE_DATA_BUCKET_NAME : module.scraper_results_s3_bucket.bucket_name
      DESTINATION_DATA_BUCKET_NAME : module.aggregated_results_s3_bucket.bucket_name
      DESTINATION_DATA_DISTRIBUTION_ID : aws_cloudfront_distribution.s3_distribution.id
      NEWJOBS_BUCKET_NAME : aws_s3_bucket.new_jobs.bucket
    }
  }

  tracing_config {
    mode = "Active"
  }

  tags = {
    Name : local.aggregator_lambda_name
  }

}

resource "aws_iam_role" "aggregator_lambda" {
  assume_role_policy = data.aws_iam_policy_document.allow_lambda_assumerole.json
  name               = local.aggregator_iam_role_name
  tags = {
    Name : local.aggregator_iam_role_name
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
    resources = ["${module.scraper_results_s3_bucket.bucket_arn}/*"]
  }
  statement {
    actions   = ["s3:PutObject", "s3:GetObject"]
    effect    = "Allow"
    resources = ["${module.aggregated_results_s3_bucket.bucket_arn}/*"]
  }
  statement {
    actions   = ["s3:ListBucket"]
    effect    = "Allow"
    resources = [module.scraper_results_s3_bucket.bucket_arn]
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
  statement {
    actions   = ["s3:PutObject"]
    effect    = "Allow"
    resources = ["${aws_s3_bucket.new_jobs.arn}/*"]
  }
}

resource "aws_lambda_event_source_mapping" "aggregator_sqs_trigger" {
  event_source_arn = aws_sqs_queue.aggregator_trigger.arn
  function_name    = aws_lambda_function.aggregator.arn
  batch_size       = 1
}







