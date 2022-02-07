resource "aws_iam_policy_attachment" "basic_lambda" {
  name       = "jobs-policyattachement-basiclambdarole"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
  roles = [
    aws_iam_role.aggregator_lambda.name,
    aws_iam_role.bot_role.name
  ]
}
resource "aws_iam_policy_attachment" "xray_lambda" {
  name       = "jobs-policyattachement-xraylambdarole"
  policy_arn = "arn:aws:iam::aws:policy/AWSXRayDaemonWriteAccess"
  roles = [
    aws_iam_role.aggregator_lambda.name,
    aws_iam_role.bot_role.name
  ]
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
