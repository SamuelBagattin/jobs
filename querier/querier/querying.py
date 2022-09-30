from botocore.client import BaseClient


def get_companies(s3_client: BaseClient, bucket_name: str, key: str) -> any:
    resp = s3_client.select_object_content(
        Bucket=bucket_name,
        Key=key,
        ExpressionType='SQL',
        Expression="SELECT Company.CompanyNamer FROM s3object[*].Companies.Companies[*] Company",
        InputSerialization={'JSON': {"Type": "DOCUMENT"}, "CompressionType": "NONE"},
        OutputSerialization={'JSON': {}},
    )

    for event in resp['Payload']:
        if 'Records' in event:
            records = event['Records']['Payload'].decode('utf-8')
            print(records)
        elif 'Stats' in event:
            statsDetails = event['Stats']['Details']
            print("Stats details bytesScanned: ")
            print(statsDetails['BytesScanned'])
            print("Stats details bytesProcessed: ")
            print(statsDetails['BytesProcessed'])
            print("Stats details bytesReturned: ")
            print(statsDetails['BytesReturned'])
