remote_state {
  backend = "s3"
  config = {
    bucket         = "jobs-scraper-terraform-state-paris"
    key            = "${path_relative_to_include()}/terraform.tfstate"
    region         = "eu-west-3"
    encrypt        = true
    dynamodb_table = "jobs-scraper-terraform-lock-paris"
  }
}
