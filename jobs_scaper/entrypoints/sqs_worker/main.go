package main

import (
	"fmt"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/service/sns"
	"github.com/aws/aws-sdk-go/service/sqs"
	"github.com/aws/aws-sdk-go/service/ssm"
	log "github.com/sirupsen/logrus"
	"jobs_scaper/pkg/aws_sts"
	"jobs_scaper/pkg/env"
	"jobs_scaper/pkg/post_scraping"
	"jobs_scaper/pkg/scraping"
	"jobs_scaper/pkg/upload"
)

func main() {

	log.SetLevel(log.TraceLevel)
	log.Debug("Init")

	sess := aws_sts.InitSession()
	ssmClient := ssm.New(sess)
	sqsClient := sqs.New(sess)
	snsClient := sns.New(sess)

	s3ScrapingClient := upload.NewS3Client(sess)

	for {
		var sqsQueueUrl = env.GetSqsQueueUrl()
		log.Trace("Polling")
		message, err := sqsClient.ReceiveMessage(&sqs.ReceiveMessageInput{
			QueueUrl:            sqsQueueUrl,
			MaxNumberOfMessages: aws.Int64(1),
			MessageAttributeNames: []*string{
				aws.String("All"),
			},
			WaitTimeSeconds: aws.Int64(20),
		},
		)
		if err != nil {
			panic(err)
		}
		for _, m := range message.Messages {
			fmt.Printf("%v", m)

			scrapingRes := scraping.ScrapeAll(ssmClient, s3ScrapingClient)

			deleteMessage, err := sqsClient.DeleteMessage(&sqs.DeleteMessageInput{
				QueueUrl:      sqsQueueUrl,
				ReceiptHandle: message.Messages[0].ReceiptHandle,
			})
			if err != nil {
				panic(err)
			}
			fmt.Printf("%v", deleteMessage)

			postScrapingErr := post_scraping.SendResultToSns(scrapingRes, snsClient)
			if postScrapingErr != nil {
				return
			}
		}
	}
}
