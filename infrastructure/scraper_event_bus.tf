resource "aws_sns_topic" "scraper_event_bus" {
  name = local.scraping_destination_sns_topic_name


  tags = {
    Project: local.project_name
  }
}

//resource "aws_lambda_function_event_invoke_config" "scraper_to_eventbus" {
//  function_name = aws_lambda_function.scraper.function_name
//  destination_config {
//    on_failure {
//      destination = ""
//    }
//  }
//}
