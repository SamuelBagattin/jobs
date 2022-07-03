resource "aws_s3_bucket" "s3_bucket" {
  bucket = local.bucket_name
}

resource "aws_s3_bucket_versioning" "s3_bucket_versioning" {
  bucket = aws_s3_bucket.s3_bucket.id
  versioning_configuration {
    status = "Disabled"
  }
}

resource "aws_s3_bucket_server_side_encryption_configuration" "sse" {
  bucket = aws_s3_bucket.s3_bucket.id

  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
  }
}

resource "aws_s3_bucket_logging" "s3_bucket_encrypt_logging" {
  count  = local.access_logs_bucket_name == null ? 0 : 1
  bucket = aws_s3_bucket.s3_bucket.id

  target_bucket = local.access_logs_bucket_name
  target_prefix = "${local.bucket_name}/"
}

resource "aws_s3_bucket_lifecycle_configuration" "s3_life_cycle_rule" {
  bucket = aws_s3_bucket.s3_bucket.id

  rule {
    status = "Enabled"

    id = "${local.bucket_name}-lifecycle-rule"

    dynamic "expiration" {
      for_each = local.objects_expiration_days == null ? [] : ["DUMMY"]
      content {
        days = local.objects_expiration_days
      }
    }

    abort_incomplete_multipart_upload {
      days_after_initiation = 7
    }
  }
}

resource "aws_s3_bucket_policy" "s3_bucket" {
  count  = local.set_bucket_policy ? 1 : 0
  bucket = aws_s3_bucket.s3_bucket.id

  policy = local.bucket_policy
}

resource "aws_s3_bucket_ownership_controls" "s3_bucket" {
  bucket = aws_s3_bucket.s3_bucket.id

  rule {
    object_ownership = "BucketOwnerEnforced"
  }

  depends_on = [
    aws_s3_bucket_policy.s3_bucket
  ]
}

resource "aws_s3_bucket_public_access_block" "s3_bucket" {
  bucket = aws_s3_bucket.s3_bucket.id

  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true

  depends_on = [
    aws_s3_bucket_ownership_controls.s3_bucket
  ]
}