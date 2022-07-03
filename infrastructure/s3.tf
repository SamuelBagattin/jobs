module "aggregated_results_s3_bucket" {
  source            = "./modules/s3_bucket"
  bucket_name       = local.aggregator_results_bucket_name
  bucket_policy     = data.aws_iam_policy_document.s3_aggregated_results_policy.json
  set_bucket_policy = true
}

data "aws_iam_policy_document" "s3_aggregated_results_policy" {
  statement {
    actions   = ["s3:GetObject"]
    resources = ["${module.aggregated_results_s3_bucket.bucket_arn}/*"]

    principals {
      type        = "AWS"
      identifiers = [aws_cloudfront_origin_access_identity.origin_access_identity.iam_arn]
    }
  }
}

resource "aws_cloudfront_origin_access_identity" "origin_access_identity" {
  comment = "Some comment"
}

module "lambda_deployments_s3_bucket" {
  source      = "./modules/s3_bucket"
  bucket_name = local.lambda_deployments_bucket_name
}

module "access_logs_s3_bucket" {
  source                  = "./modules/s3_bucket"
  bucket_name             = "jobs-s3accesslogs-bucket"
  objects_expiration_days = 30
  bucket_policy           = data.aws_iam_policy_document.s3_bucket_policy.json
  set_bucket_policy       = true
}

data "aws_iam_policy_document" "s3_bucket_policy" {
  statement {
    sid    = "S3ServerAccessLogsPolicy"
    effect = "Allow"
    principals {
      identifiers = ["logging.s3.amazonaws.com"]
      type        = "Service"
    }
    actions   = ["s3:PutObject"]
    resources = ["${module.access_logs_s3_bucket.bucket_arn}/*"]
    condition {
      test     = "StringEquals"
      values   = [data.aws_caller_identity.identity.account_id]
      variable = "aws:SourceAccount"
    }
  }
}
