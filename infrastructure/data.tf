data "aws_route53_zone" "samuelbagattin" {
  name = local.base_domain
}

data "aws_acm_certificate" "samuelbagattin" {
  domain   = local.base_domain
}
