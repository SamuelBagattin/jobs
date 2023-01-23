import * as cdk from 'aws-cdk-lib';
import { Construct } from 'constructs';

const staticInputs = {
  bucket: `jobs-website-bucket`,
  distribution: {
    id: `E3B4EZ17897IFP`,
    domainName: `d3igo64g9086ea.cloudfront.net`,
  },
};

export class InfrastructureStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    new cdk.aws_s3_deployment.BucketDeployment(this, `DeployWebsite`, {
      sources: [cdk.aws_s3_deployment.Source.asset(`../out`)],
      destinationBucket: cdk.aws_s3.Bucket.fromBucketName(
        this,
        `WebsiteBucket`,
        staticInputs.bucket,
      ),
      distribution: cdk.aws_cloudfront.Distribution.fromDistributionAttributes(
        this,
        `WebsiteDistribution`,
        {
          distributionId: staticInputs.distribution.id,
          domainName: staticInputs.distribution.domainName,
        },
      ),
      distributionPaths: [`/*`],
    });
  }
}
