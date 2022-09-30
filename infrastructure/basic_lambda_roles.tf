locals {
  lambdas_roles_names = [
    aws_iam_role.aggregator_lambda.name,
    aws_iam_role.bot_role.name,
    aws_iam_role.scraper.name,
    #    aws_iam_role.querier_role.name
  ]
}

resource "aws_iam_role_policy_attachment" "basic_lambda" {
  for_each   = { for role in local.lambdas_roles_names : role => role }
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
  role       = each.value
}
resource "aws_iam_role_policy_attachment" "xray_lambda" {
  for_each   = { for role in local.lambdas_roles_names : role => role }
  policy_arn = "arn:aws:iam::aws:policy/AWSXRayDaemonWriteAccess"
  role       = each.value
}

data "aws_iam_policy_document" "allow_lambda_assumerole" {
  version = "2012-10-17"
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      identifiers = ["lambda.amazonaws.com"]
      type        = "Service"
    }
    effect = "Allow"
  }
}
