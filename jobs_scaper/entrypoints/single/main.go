package main

import (
	"github.com/aws/aws-sdk-go/service/sns"
	"github.com/aws/aws-sdk-go/service/ssm"
	log "github.com/sirupsen/logrus"
	"jobs_scaper/pkg/aws_sts"
	"jobs_scaper/pkg/post_scraping"
	"jobs_scaper/pkg/scraping"
	"jobs_scaper/pkg/upload"
)

func main() {
	log.SetLevel(log.TraceLevel)
	log.Debug("Init")

	sess := aws_sts.InitSession()
	ssmClient := ssm.New(sess)
	snsClient := sns.New(sess)

	s3ScrapingClient := upload.NewS3Client(sess)

	scrapingRes := scraping.ScrapeAll(ssmClient, s3ScrapingClient)

	postScrapingErr := post_scraping.SendResultToSns(scrapingRes, snsClient)
	if postScrapingErr != nil {
		return
	}
}
