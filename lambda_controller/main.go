package main

import (
	"encoding/json"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/lambda"
	"github.com/aws/aws-sdk-go/service/ssm"
	log "github.com/sirupsen/logrus"
	"sync"
	"time"
)

func main() {
	log.SetLevel(log.TraceLevel)
	log.Debug("Init")
	sess := session.Must(session.NewSession(&aws.Config{
		Region: aws.String("eu-west-3"),
	}))
	ssmClient := ssm.New(sess)
	lambdaClient := lambda.New(sess)

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
	log.WithField("queries", queries).Debug()

	var wg sync.WaitGroup
	for _, query := range queries {
		queryBody := LambdaEvent{Query: query}
		res, marshalErr := json.Marshal(queryBody)
		if marshalErr != nil {
			panic(marshalErr)
		}
		wg.Add(1)
		query := query
		go func() {
			log.WithField("lambda name", "jobs-scraper-lambda").WithField("query", query).Debug()
			lambdaResponse, err := lambdaClient.Invoke(&lambda.InvokeInput{
				FunctionName: aws.String("jobs-scraper-lambda"),
				Payload:      res,
			})
			if err != nil {
				log.WithFields(log.Fields{
					"lambda name": "jobs-scraper-lambda",
					"query":       query,
				}).Error("Error while calling lambda")
			}
			wg.Done()
			if err == nil {
				log.WithFields(log.Fields{
					"lambda name": "jobs-scraper-lambda",
					"query":       query,
					"body":        lambdaResponse,
				}).Info("Success !!!")
			}
		}()
		time.Sleep(time.Second * 2)
	}
	wg.Wait()

}

type LambdaEvent struct {
	Query string
}
