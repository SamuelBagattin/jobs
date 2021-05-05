package env

import "github.com/aws/aws-sdk-go/aws"

const (
	IamRoleToAssumeEnvVarName = "IAM_ROLE"
)

func GetAssumeRole() *string {
	return getEnvVar(aws.String(IamRoleToAssumeEnvVarName))
}
