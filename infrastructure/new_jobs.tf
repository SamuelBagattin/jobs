resource "aws_s3_bucket" "new_jobs" {
  bucket = local.newjobs_history_bucket_name
  acl    = "private"
  versioning {
    enabled = true
  }
  tags = {
    Project : local.project_name
    Name : local.newjobs_history_bucket_name
  }
}

