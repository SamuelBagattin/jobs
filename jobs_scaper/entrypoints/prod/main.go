package main

import (
	"encoding/json"
	"fmt"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"jobs_scaper/pkg/scraping"
	"jobs_scaper/pkg/upload"
	"log"
	"sync"
)

func main() {

	defer func() {
		if err := recover(); err != nil {
			fmt.Println(err)
		}
	}()
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

	sess := session.Must(session.NewSession(&aws.Config{
		Region: aws.String("us-east-1"),
	}))

	var s3ScrapClient = upload.NewS3Client(sess)

	var wg sync.WaitGroup

	for _, client := range scrapingClients {
		wg.Add(1)
		client := client
		go func() {
			defer wg.Done()
			res, err := client.Scrape()
			if err != nil {
				panic(err)
			}

			b, err := json.Marshal(res)
			if err != nil {
				log.Println("failed to serialize response:", err)
				return
			}
			s3ScrapClient.UploadToFileByte(client.GetConfig().SiteName, b)
		}()
	}
	wg.Wait()

}
