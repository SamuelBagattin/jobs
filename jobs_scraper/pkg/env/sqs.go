package env

import "github.com/aws/aws-sdk-go/aws"

const (
	SqsQueueUrlEnvVarName = "SQS_QUEUE_URL"
)

func GetSqsQueueUrl() *string {
	return getEnvVar(aws.String(SqsQueueUrlEnvVarName))
}
