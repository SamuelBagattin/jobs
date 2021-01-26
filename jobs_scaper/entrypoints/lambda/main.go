package main

import (
	"github.com/aws/aws-lambda-go/lambda"
	"jobs_scaper/pkg"
)

func main() {
	indeedUrl := "https://fr.indeed.com/jobs?q=Developpeur&l=Bordeaux%20(33)&radius=5"

	lambda.Start(func() error {
		var jobs = pkg.ScrapeIndeed(indeedUrl)
		err := pkg.InsertJobsIntoPostgresDb(jobs)
		if err != nil {
			panic(err)
		}
return nil
	})
}
