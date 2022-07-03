data "aws_ssm_parameter" "github_token" {
  name            = "/github/samuelbagattin/token"
  with_decryption = true
}

provider "aws" {
  region  = "eu-west-3"
  profile = "samuel"
  default_tags {
    tags = {
      Project : "jobs"
    }
  }
}

provider "aws" {
  region  = "us-east-1"
  alias   = "nvirignia"
  profile = "samuel"
}

provider "github" {
  token = data.aws_ssm_parameter.github_token.value
}

provider "archive" {}

terraform {
  required_version = ">=1"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
    github = {
      source  = "integrations/github"
      version = "~> 4"
    }
    archive = {
      source  = "hashicorp/archive"
      version = "~> 2"
    }
  }
  backend "s3" {}
}
