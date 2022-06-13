resource "aws_route53_record" "jobs_samuelbagattin_com" {
  name    = local.website_domain
  type    = "A"
  zone_id = data.aws_route53_zone.samuelbagattin.id
  alias {
    evaluate_target_health = false
    name                   = aws_cloudfront_distribution.s3_distribution.domain_name
    zone_id                = aws_cloudfront_distribution.s3_distribution.hosted_zone_id
  }
}

resource "aws_route53_record" "jobs_samuelbagattin_com_ipv6" {
  name    = local.website_domain
  type    = "AAAA"
  zone_id = data.aws_route53_zone.samuelbagattin.id
  alias {
    evaluate_target_health = false
    name                   = aws_cloudfront_distribution.s3_distribution.domain_name
    zone_id                = aws_cloudfront_distribution.s3_distribution.hosted_zone_id
  }
}
