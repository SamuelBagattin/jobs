package main

import (
	"github.com/aws/aws-sdk-go/service/ssm"
	log "github.com/sirupsen/logrus"
	"jobs_scaper/pkg/aws_sts"
	"jobs_scaper/pkg/scraping"
	"jobs_scaper/pkg/upload"
)

func main() {
	log.SetLevel(log.TraceLevel)
	log.Debug("Init")

	sess := aws_sts.InitSession()
	ssmClient := ssm.New(sess)

	s3ScrapingClient := upload.NewS3Client(sess)

	scraping.ScrapeAll(ssmClient, s3ScrapingClient)
}
