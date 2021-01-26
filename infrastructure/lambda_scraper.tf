module "jobs_scraper" {
  source = "terraform-aws-modules/lambda/aws"

  function_name = "jobs-scraper"
  description   = "Lambda function for jobs scraping"
  handler       = "lambda_entrypoint"
  runtime       = "go1.x"

  create_package         = false
  local_existing_package = "../lambda_entrypoint.zip"
}
