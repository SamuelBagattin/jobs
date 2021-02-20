resource "aws_s3_bucket" "scraper_results" {
  bucket = "jobs-scraper-results"
  acl = "private"
  tags = {
    Name = "scraper-results"
    Project = "Jobs Scraper"
  }
}
resource "aws_s3_bucket" "aggregated_results" {
  bucket = "jobs-aggregator-results"
  acl = "private"
  tags = {
    Name = "jobs-aggregator-results"
    Project = "Jobs Scraper"
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
