package scraping

import (
	"encoding/json"
	"fmt"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/service/ssm"
	log "github.com/sirupsen/logrus"
	"jobs_scaper/pkg/upload"
	"net/url"
	"sync"
	"time"
)

type JobInfo struct {
	Title       string
	Company     string
	Site        string
	Url         string
	Date        time.Time
	Id          string
	Description string
}

type ClientConfig struct {
	SiteName     string
	SiteOrigin   string
	SiteBasePath *url.URL
}

type Client interface {
	GetConfig() *ClientConfig
	Scrape(query string) (*[]*JobInfo, error)
	logWithName(msg string) string
}

type FinalScrapingResult struct {
	QueriesStatistics map[string]*int `json:"queries_statistics"`
}

func ScrapeAll(ssmClient *ssm.SSM, s3ScrapClient *upload.S3ScrapingClient) *FinalScrapingResult {
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
	return &FinalScrapingResult{QueriesStatistics: queriesSummary}
}

func scrapeQuery(query string, s3ScrapClient *upload.S3ScrapingClient) *int {
	defer func() {
		if err := recover(); err != nil {
			fmt.Println(err)
		}
	}()
	log.SetLevel(log.TraceLevel)

	var scrapingClients []Client

	var linkedInClient, nLinkedInClientErr = NewLinkedinClient()
	if nLinkedInClientErr != nil {
		panic(nLinkedInClientErr)
	}

	scrapingClients = append(scrapingClients, linkedInClient)

	var monsterClient, nMonsterClientErr = NewMonsterClient()
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
