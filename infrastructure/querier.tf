#
#resource "aws_api_gateway_rest_api" "querier" {
#  name = local.querier_api_gateway_name
#  # multiline string
#  body = jsonencode({
#    "openapi" : "3.0.1",
#    "info" : {
#      "title" : local.querier_api_gateway_name,
#      "version" : "2022-09-26T17:09:17Z"
#    },
#    "paths" : {
#      "/{proxy+}" : {
#        "get" : {
#          "parameters" : [{
#            "name" : "proxy",
#            "in" : "path",
#            "required" : true,
#            "schema" : {
#              "type" : "string"
#            }
#          }],
#          "responses" : {
#            "200" : {
#              "description" : "200 response",
#              "content" : {
#                "application/json" : {
#                  "schema" : {
#                    "$ref" : "#/components/schemas/Empty"
#                  }
#                }
#              }
#            }
#          },
#          "x-amazon-apigateway-integration" : {
#            "httpMethod" : "POST",
#            "uri" : aws_lambda_function.querier.invoke_arn,
#            "responses" : {
#              "default" : {
#                "statusCode" : "200"
#              }
#            },
#            "passthroughBehavior" : "when_no_match",
#            "cacheKeyParameters" : ["method.request.path.proxy"],
#            "contentHandling" : "CONVERT_TO_TEXT",
#            "type" : "aws_proxy"
#          }
#        }
#      }
#    },
#    "components" : {
#      "schemas" : {
#        "Empty" : {
#          "title" : "Empty Schema",
#          "type" : "object"
#        }
#      }
#    }
#  })
#}
#
#resource "aws_api_gateway_deployment" "querier_prod" {
#  rest_api_id = aws_api_gateway_rest_api.querier.id
#}
#
#resource "aws_api_gateway_stage" "querier_prod" {
#  deployment_id = aws_api_gateway_deployment.querier_prod.id
#  rest_api_id   = aws_api_gateway_rest_api.querier.id
#  stage_name    = "prod"
#}
#
#data "archive_file" "querier" {
#  type = "zip"
#
#  source_dir  = "${path.root}/../querier/package"
#  output_path = "${path.root}/../out/zip/querier/querier.zip"
#}
#
#resource "aws_s3_object" "querier" {
#  bucket = module.lambda_deployments_s3_bucket.bucket_name
#
#  key    = "querier.zip"
#  source = data.archive_file.querier.output_path
#
#  etag = filemd5(data.archive_file.querier.output_path)
#}
#
#resource "aws_lambda_function" "querier" {
#  function_name    = local.querier_lambda_name
#  handler          = "querier/lambda_function.lambda_handler"
#  role             = aws_iam_role.querier_role.arn
#  runtime          = "python3.9"
#  architectures    = ["x86_64"]
#  s3_bucket        = aws_s3_object.querier.bucket
#  s3_key           = aws_s3_object.querier.key
#  source_code_hash = data.archive_file.querier.output_base64sha256
#  timeout          = 30
#  memory_size      = 128
#  layers           = ["arn:aws:lambda:eu-west-3:017000801446:layer:AWSLambdaPowertoolsPython:36"]
#
#  environment {
#    variables = {
#      DATA_BUCKET_NAME = module.aggregated_results_s3_bucket.bucket_name
#    }
#  }
#
#  tracing_config {
#    mode = "Active"
#  }
#
#  tags = {
#    Project : local.project_name
#    Name : local.querier_lambda_name
#  }
#
#}
#
#resource "aws_iam_role" "querier_role" {
#  name = local.querier_lambda_role_name
#
#  assume_role_policy = data.aws_iam_policy_document.allow_lambda_assumerole.json
#}
#
#resource "aws_iam_policy_attachment" "querier_policy_to_role" {
#  policy_arn = aws_iam_policy.querier_policy.arn
#  roles      = [aws_iam_role.querier_role.name]
#  name       = local.querier_lambda_policy_attachment_name
#}
#
#resource "aws_iam_policy" "querier_policy" {
#  policy = data.aws_iam_policy_document.querier_policy.json
#  name   = local.querier_iam_policy_name
#}
#
#data "aws_iam_policy_document" "querier_policy" {
#  version = "2012-10-17"
#  statement {
#    actions = ["s3:GetObject"]
#    effect  = "Allow"
#    resources = [
#      "${module.scraper_results_s3_bucket.bucket_arn}/index.json",
#    ]
#  }
#}
#
#resource "aws_lambda_permission" "querier_apigateway" {
#  action        = "lambda:InvokeFunction"
#  function_name = aws_lambda_function.querier.function_name
#  principal     = "apigateway.amazonaws.com"
#  source_arn    = "${aws_api_gateway_rest_api.querier.execution_arn}/*/*/*"
#}