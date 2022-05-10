resource "aws_s3_bucket" "new_jobs" {
  bucket = local.newjobs_history_bucket_name
  tags = {
    Project : local.project_name
    Name : local.newjobs_history_bucket_name
  }
}

resource "aws_s3_bucket_versioning" "new_jobs" {
  bucket = aws_s3_bucket.new_jobs.bucket
  versioning_configuration {
    status = "Enabled"
  }
}

resource "aws_s3_bucket_acl" "new_jobs" {
  bucket = aws_s3_bucket.new_jobs.id
  acl    = "private"
}
