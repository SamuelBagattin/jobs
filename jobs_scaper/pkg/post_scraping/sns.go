package post_scraping

import (
	"encoding/json"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/service/sns"
	"jobs_scaper/pkg/env"
	"jobs_scaper/pkg/scraping"
)

func SendResultToSns(queriesSummary *scraping.FinalScrapingResult, snsClient *sns.SNS) error {
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
