resource "aws_cloudfront_distribution" "s3_distribution" {
  http_version = "http3"
  origin {
    domain_name = module.aggregated_results_s3_bucket.bucket_regional_domain_name
    origin_id   = local.website_origin_id

    s3_origin_config {
      origin_access_identity = aws_cloudfront_origin_access_identity.origin_access_identity.cloudfront_access_identity_path
    }
  }

  enabled             = true
  is_ipv6_enabled     = true
  comment             = local.website_distribution_name
  default_root_object = "index.html"

  aliases = [local.website_domain]
  viewer_certificate {
    acm_certificate_arn      = data.aws_acm_certificate.samuelbagattin.arn
    minimum_protocol_version = "TLSv1.2_2021"
    ssl_support_method       = "sni-only"
  }

  default_cache_behavior {
    allowed_methods  = ["GET", "HEAD", "OPTIONS"]
    cached_methods   = ["GET", "HEAD", "OPTIONS"]
    target_origin_id = local.website_origin_id
    compress         = true
    forwarded_values {
      query_string = false

      cookies {
        forward = "none"
      }
    }
    response_headers_policy_id = aws_cloudfront_response_headers_policy.s3.id

    viewer_protocol_policy = "redirect-to-https"
    min_ttl                = 0
    default_ttl            = 3600
    max_ttl                = 86400
  }

  price_class = "PriceClass_100"

  wait_for_deployment = true

  restrictions {
    geo_restriction {
      restriction_type = "whitelist"
      locations        = ["FR"]
    }
  }
}

resource "aws_cloudfront_response_headers_policy" "s3" {
  name = "s3"
  cors_config {
    access_control_allow_credentials = false
    origin_override                  = false
    access_control_allow_headers {
      items = ["*"]
    }
    access_control_allow_methods {
      items = ["GET", "HEAD", "OPTIONS"]
    }
    access_control_allow_origins {
      items = ["*"]
    }
  }
}
