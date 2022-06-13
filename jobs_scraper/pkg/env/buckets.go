package env

import (
	"github.com/aws/aws-sdk-go/aws"
)

const (
	ResultsBucketEnvVarName = "RESULTS_BUCKET_NAME"
)

func GetUploadBucketName() *string {
	return getEnvVar(aws.String(ResultsBucketEnvVarName))
}
