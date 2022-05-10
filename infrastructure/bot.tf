resource "aws_lambda_function" "bot" {
  function_name = local.discord_bot_lambda_name
  handler       = "main.handler"
  role          = aws_iam_role.bot_role.arn
  runtime       = "nodejs14.x"
  architectures = ["x86_64"]
  filename      = "bot.zip"
  timeout       = 30
  memory_size   = 128

  environment {
    variables = {
      ON_LAMBDA : true
      TOKEN_SSM_PARAM_NAME : data.aws_ssm_parameter.discord_bot_token.name
      PASTEEE_TOKEN_SSM_PARAM_NAME : data.aws_ssm_parameter.paste_ee_token.name
    }
  }

  tracing_config {
    mode = "Active"
  }

  tags = {
    Project : local.project_name
    Name : local.discord_bot_lambda_name
  }

}

resource "aws_iam_role" "bot_role" {
  name = local.discord_bot_role_name

  assume_role_policy = data.aws_iam_policy_document.allow_lambda_assumerole.json
}

resource "aws_iam_policy_attachment" "discordbot_policy_to_role" {
  name       = local.discord_bot_policy_attachment_name
  policy_arn = aws_iam_policy.discord_bot_policy.arn
  roles      = [aws_iam_role.bot_role.name]
}

resource "aws_iam_policy" "discord_bot_policy" {
  policy = data.aws_iam_policy_document.discord_bot_policy.json
  name   = local.discord_bot_iam_policy_name
}

data "aws_iam_policy_document" "discord_bot_policy" {
  version = "2012-10-17"
  statement {
    actions   = ["ssm:GetParameter"]
    effect    = "Allow"
    resources = [data.aws_ssm_parameter.discord_bot_token.arn, data.aws_ssm_parameter.paste_ee_token.arn]
  }
  statement {
    actions   = ["s3:GetObject"]
    effect    = "Allow"
    resources = ["${aws_s3_bucket.new_jobs.arn}/*"]
  }
}

resource "aws_lambda_permission" "allow_bucket_bot" {
  statement_id  = "AllowExecutionFromS3Bucket"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.bot.arn
  principal     = "s3.amazonaws.com"
  source_arn    = aws_s3_bucket.new_jobs.arn
}


resource "aws_s3_bucket_notification" "bucket_notification" {
  bucket = aws_s3_bucket.new_jobs.id

  lambda_function {
    lambda_function_arn = aws_lambda_function.bot.arn
    events              = ["s3:ObjectCreated:*"]
  }

  depends_on = [aws_lambda_permission.allow_bucket_bot]
}

data "aws_ssm_parameter" "discord_bot_token" {
  name        = local.discord_bot_token_secret_name
}

data "aws_ssm_parameter" "paste_ee_token" {
  name        = local.discord_bot_paste_ee_token_secret_name
}
