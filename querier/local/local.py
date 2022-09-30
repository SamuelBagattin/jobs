import os

import boto3
from aws_lambda_powertools.utilities.data_classes import APIGatewayProxyEvent
from aws_lambda_powertools.utilities.typing import LambdaContext

from querier import lambda_function, router

session = boto3.session.Session()
s3 = session.client('s3')

router.handle_event("/companies", s3, "jobs-aggregator-results")
