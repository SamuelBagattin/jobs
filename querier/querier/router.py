import json

from botocore.client import BaseClient

from querier.querying import get_companies


def handle_event(message_path: str, s3_client: BaseClient, data_bucket_name: str) -> dict:
    match message_path:
        case "/companies":
            companies = get_companies(s3_client, data_bucket_name, "index.json")
            return {
                'statusCode': '200',
                'body': json.dumps({
                    'message': companies
                })
            }
    message = 'Hello world!'
    return {
        'statusCode': '200',
        'headers': {
            "x-custom-header": "my custom header value"
        },
        'body': json.dumps({
            'message': message
        })
    }
