package env

import "github.com/aws/aws-sdk-go/aws"

const (
	SnsTopicDestinationArnEnvVarName = "SNS_TOPIC_DESTINATION_ARN"
)

func GetSnsTopicDestinationArn() *string {
	return getEnvVar(aws.String(SnsTopicDestinationArnEnvVarName))
}
