remote_state {
  backend = "s3"
  config = {
    bucket = "jobs-scraper-terraform-state"
    key            = "${path_relative_to_include()}/terraform.tfstate"
    region         = "eu-central-1"
    encrypt        = true
    dynamodb_table = "jobs-scraper-terraform-lock"
  }
}
