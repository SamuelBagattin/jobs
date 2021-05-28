package aws_sts

import (
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/credentials/stscreds"
	"github.com/aws/aws-sdk-go/aws/session"
	"jobs_scaper/pkg/env"
)

func InitSession() *session.Session {
	var awsRegion = env.GetAwsRegion()
	sess := session.Must(session.NewSession(&aws.Config{
		Region: awsRegion,
	}))

	roleToAssumeArn := env.GetAssumeRole()
	assumedCreds := stscreds.NewCredentials(sess, *roleToAssumeArn, func(p *stscreds.AssumeRoleProvider) {})

	return session.Must(session.NewSession(&aws.Config{
		Region:      awsRegion,
		Credentials: assumedCreds,
	}))
}
