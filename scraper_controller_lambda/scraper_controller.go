package main

import (
	"fmt"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/awserr"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/ec2"
	"github.com/aws/aws-sdk-go/service/ssm"
)

func startScraper() {
	sess := session.Must(session.NewSession())
	config := &aws.Config{
		Region: aws.String("us-east-1"),
	}
	ec2Client := ec2.New(sess, config)
	ssmClient := ssm.New(sess, config)

	ami, err := ssmClient.GetParameter(&ssm.GetParameterInput{Name: aws.String("/aws/service/ami-amazon-linux-latest/amzn2-ami-hvm-x86_64-gp2")})
	if err != nil {
		panic(err)
	}

	input := &ec2.RunInstancesInput{
		BlockDeviceMappings: []*ec2.BlockDeviceMapping{
			{
				DeviceName: aws.String("/dev/xvda"),
				Ebs: &ec2.EbsBlockDevice{
					VolumeSize: aws.Int64(8),
				},
			},
		},
		ImageId:      ami.Parameter.Value,
		InstanceType: aws.String("t3a.micro"),
		KeyName:      aws.String("EC2TutorialNVirginia"),
		MaxCount:     aws.Int64(1),
		MinCount:     aws.Int64(1),
		SecurityGroupIds: []*string{
			aws.String("sg-06e8712a"),
		},
		SubnetId: aws.String("subnet-387f9467"),
		TagSpecifications: []*ec2.TagSpecification{
			{
				ResourceType: aws.String("instance"),
				Tags: []*ec2.Tag{
					{
						Key:   aws.String("Purpose"),
						Value: aws.String("test"),
					},
				},
			},
		},
	}

	result, err := ec2Client.RunInstances(input)
	if err != nil {
		if aerr, ok := err.(awserr.Error); ok {
			switch aerr.Code() {
			default:
				fmt.Println(aerr.Error())
			}
		} else {
			// Print the error, cast err to awserr.Error to get the Code and
			// Message from an error.
			fmt.Println(err.Error())
		}
		return
	}

	fmt.Println(result)

}
