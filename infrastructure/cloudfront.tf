resource "aws_cloudfront_distribution" "s3_distribution" {
  http_version = "http3"
  origin {
    domain_name              = module.aggregated_results_s3_bucket.bucket_regional_domain_name
    origin_id                = local.aggregator_cloudfront_origin_id
    origin_access_control_id = aws_cloudfront_origin_access_control.aggregator_results.id
  }
  origin {
    domain_name              = module.website_s3_bucket.bucket_regional_domain_name
    origin_id                = local.website_origin_id
    origin_access_control_id = aws_cloudfront_origin_access_control.website.id
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

  ordered_cache_behavior {
    allowed_methods            = ["GET", "HEAD", "OPTIONS"]
    cached_methods             = ["GET", "HEAD", "OPTIONS"]
    path_pattern               = "/api/*"
    target_origin_id           = local.aggregator_cloudfront_origin_id
    viewer_protocol_policy     = "redirect-to-https"
    min_ttl                    = 86400
    default_ttl                = 86400
    max_ttl                    = 86400
    response_headers_policy_id = aws_cloudfront_response_headers_policy.s3.id
    compress                   = true
    forwarded_values {
      query_string = false

      cookies {
        forward = "none"
      }
    }
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
    viewer_protocol_policy = "redirect-to-https"
    min_ttl                = 86400
    max_ttl                = 86400
    default_ttl            = 86400
    function_association {
      event_type   = "viewer-request"
      function_arn = aws_cloudfront_function.uri_indexhtml_appender.arn
    }
  }
  custom_error_response {
    error_code            = 403
    error_caching_min_ttl = 60
    response_code         = 404
    response_page_path    = "/404.html"
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

resource "aws_cloudfront_function" "uri_indexhtml_appender" {
  code    = file("cloudfront/uri_indexhtml_appender.js")
  name    = "jobs-uriIndexhtmlAppender-function"
  runtime = "cloudfront-js-1.0"
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
