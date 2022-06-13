resource "aws_s3_bucket" "aggregated_results" {
  bucket = local.aggregator_results_bucket_name
  acl    = "private"
  tags = {
    Name    = local.aggregator_results_bucket_name
    Project = local.project_name
  }
}

data "aws_iam_policy_document" "s3_policy" {
  statement {
    actions   = ["s3:GetObject"]
    resources = ["${aws_s3_bucket.aggregated_results.arn}/*"]

    principals {
      type        = "AWS"
      identifiers = [aws_cloudfront_origin_access_identity.origin_access_identity.iam_arn]
    }
  }
}

resource "aws_s3_bucket_policy" "example" {
  bucket = aws_s3_bucket.aggregated_results.id
  policy = data.aws_iam_policy_document.s3_policy.json
}

resource "aws_cloudfront_origin_access_identity" "origin_access_identity" {
  comment = "Some comment"
}

resource "aws_s3_bucket" "lambda_deployments" {
  bucket = local.lambda_deployments_bucket_name

  tags = {
    Name    = local.lambda_deployments_bucket_name
    Project = local.project_name
  }
}

resource "aws_s3_bucket_acl" "lambda_deployments" {
  bucket = aws_s3_bucket.lambda_deployments.id
  acl    = "private"
}

resource "aws_s3_bucket_versioning" "lambda_deployments" {
  bucket = aws_s3_bucket.lambda_deployments.id
  versioning_configuration {
    status = "Disabled"
  }
}

resource "aws_s3_bucket_server_side_encryption_configuration" "lambda_deployments" {
  bucket = aws_s3_bucket.lambda_deployments.id

  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
  }
}


resource "aws_s3_bucket_lifecycle_configuration" "lambda_deployments" {
  bucket = aws_s3_bucket.lambda_deployments.id

  rule {
    id     = "${aws_s3_bucket.lambda_deployments.bucket}-lifecycle-rule"
    status = "Enabled"
    abort_incomplete_multipart_upload {
      days_after_initiation = 7
    }
  }
}


resource "aws_s3_bucket_ownership_controls" "s3_bucket" {
  bucket = aws_s3_bucket.lambda_deployments.id

  rule {
    object_ownership = "BucketOwnerPreferred"
  }
}