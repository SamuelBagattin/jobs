import json
from os import environ

import boto3
from aws_lambda_powertools import Tracer, Logger
from aws_lambda_powertools.utilities.data_classes import APIGatewayProxyEvent, event_source
from aws_lambda_powertools.utilities.typing import LambdaContext

from querier.querying import get_companies
from querier.router import handle_event

tracer = Tracer()
logger = Logger()

bucket_name = environ.get('DATA_BUCKET_NAME')
key = "companies.json"
session = boto3.session.Session()
s3 = session.client('s3')


@tracer.capture_lambda_handler
@logger.inject_lambda_context(log_event=True)
@event_source(data_class=APIGatewayProxyEvent)
def lambda_handler(event: APIGatewayProxyEvent, context):
    return handle_event(event.path, s3, bucket_name)


