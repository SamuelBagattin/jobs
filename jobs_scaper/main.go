package main

import (
	"encoding/json"
	"fmt"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/credentials/stscreds"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/sns"
	"github.com/aws/aws-sdk-go/service/sqs"
	"github.com/aws/aws-sdk-go/service/ssm"
	log "github.com/sirupsen/logrus"
	"jobs_scaper/pkg/env"
	"jobs_scaper/pkg/scraping"
	"jobs_scaper/pkg/upload"
	"net/url"
	"sync"
)

func main() {

	log.SetLevel(log.TraceLevel)
	log.Debug("Init")

	sess := initSession()
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

			scrapingRes := scrape(ssmClient, s3ScrapingClient)

			deleteMessage, err := sqsClient.DeleteMessage(&sqs.DeleteMessageInput{
				QueueUrl:      sqsQueueUrl,
				ReceiptHandle: message.Messages[0].ReceiptHandle,
			})
			if err != nil {
				panic(err)
			}
			fmt.Printf("%v", deleteMessage)

			postScrapingErr := postScraping(scrapingRes, snsClient)
			if postScrapingErr != nil {
				return
			}
		}
	}
}

func scrape(ssmClient *ssm.SSM, s3ScrapClient *upload.S3ScrapingClient) *scraping.FinalScrapingResult {
	log.Debug("Getting jobs-scraping-queries parameter")
	param, err := ssmClient.GetParameter(&ssm.GetParameterInput{
		Name: aws.String("jobs-scraping-queries"),
	})
	if err != nil {
		panic(err)
	}

	var queries []string
	if unmarshalErr := json.Unmarshal([]byte(*param.Parameter.Value), &queries); unmarshalErr != nil {
		panic(unmarshalErr)
	}
	var queriesSummary = make(map[string]*int)
	for _, query := range queries {
		queriesSummary[query] = scrapeQuery(query, s3ScrapClient)
	}
	return &scraping.FinalScrapingResult{QueriesStatistics: queriesSummary}
}

func initSession() *session.Session {
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

func scrapeQuery(query string, s3ScrapClient *upload.S3ScrapingClient) *int {
	defer func() {
		if err := recover(); err != nil {
			fmt.Println(err)
		}
	}()
	log.SetLevel(log.TraceLevel)

	var scrapingClients []scraping.Client

	var linkedInClient, nLinkedInClientErr = scraping.NewLinkedinClient()
	if nLinkedInClientErr != nil {
		panic(nLinkedInClientErr)
	}

	scrapingClients = append(scrapingClients, linkedInClient)

	var monsterClient, nMonsterClientErr = scraping.NewMonsterClient()
	if nMonsterClientErr != nil {
		panic(nMonsterClientErr)
	}

	scrapingClients = append(scrapingClients, monsterClient)

	var wg sync.WaitGroup
	var jobsCount = 0
	for _, client := range scrapingClients {
		wg.Add(1)
		client := client
		go func() {
			defer wg.Done()
			res, err := client.Scrape(query)
			if err != nil {
				panic(err)
			}

			b, err := json.Marshal(res)
			if err != nil {
				log.Println("failed to serialize response:", err)
				return
			}
			s3ScrapClient.UploadToFileByte(client.GetConfig().SiteName+url.QueryEscape(query), b, "application/json")
			jobsCount += len(*res)
		}()
	}
	wg.Wait()
	return &jobsCount
}

func postScraping(queriesSummary *scraping.FinalScrapingResult, snsClient *sns.SNS) error {
	stringMessage, errMarshal := json.Marshal(queriesSummary)
	if errMarshal != nil {
		return errMarshal
	}
	_, err := snsClient.Publish(&sns.PublishInput{
		Message:  aws.String(string(stringMessage)),
		TopicArn: env.GetSnsTopicDestinationArn(),
	})
	if err != nil {
		return err
	}
	return nil
}
