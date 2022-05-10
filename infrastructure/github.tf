data "aws_ssm_parameter" "github_actions_oidc_provider_arn" {
  name = "githubActions-oidcProviderArn-ssmParam"
}

resource "github_repository" "jobs" {
  name                 = "jobs"
  description          = "Simplify your developer jobs findings. 👌"
  has_downloads        = true
  has_issues           = true
  has_projects         = true
  has_wiki             = true
  homepage_url         = "https://${aws_route53_record.jobs-samuelbagattin-com.name}"
  vulnerability_alerts = true
}

resource "github_actions_secret" "github_actions_iam_role_arn" {
  plaintext_value = module.aws_github_actions_oidc.roles_arns[local.githubactions_jobs_iam_role_name].iam_role_arn
  repository      = github_repository.jobs.name
  secret_name     = "IAM_ROLE_ARN"
}

module "aws_github_actions_oidc" {
  source               = "registry.terraform.io/SamuelBagattin/github-oidc-provider/aws"
  version              = "0.3.3"
  create_oidc_provider = false
  create_iam_roles     = true
  oidc_provider_arn    = data.aws_ssm_parameter.github_actions_oidc_provider_arn.value
  permissions          = {
    "SamuelBagattin" : {
      role_name : "jobs-githubActions-role"
      allowed_branches : ["coucou"]
      repositories = {
        (github_repository.jobs.name) = {
          role_name : local.githubactions_jobs_iam_role_name
          allowed_branches : ["*"]
        }
      }
    }
  }
}

resource "aws_iam_policy_attachment" "github_actions" {
  name       = "githubActions-jobs-policyAttachment"
  policy_arn = aws_iam_policy.github_actions.arn
  roles      = [local.githubactions_jobs_iam_role_name]
  depends_on = [module.aws_github_actions_oidc]
}

resource "aws_iam_policy" "github_actions" {
  policy = data.aws_iam_policy_document.github_actions.json
  name   = "githubActions-jobs-policy"
}

data "aws_iam_policy_document" "github_actions" {
  statement {
    effect  = "Allow"
    actions = [
      "lambda:UpdateFunctionCode"
    ]
    resources = [
      "*"
    ]
  }
}