package env

import "github.com/aws/aws-sdk-go/aws"

const (
	AwsRegionEnvVarName = "AWS_REGION"
)

func GetAwsRegion() *string {
	return getEnvVar(aws.String(AwsRegionEnvVarName))
}
