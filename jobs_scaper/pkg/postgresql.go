package pkg

import (
	"database/sql"
	"fmt"
	_ "github.com/lib/pq"
	"jobs_scaper/pkg/scraping"
	"os"
	"sync"
)

var once sync.Once

// variable globale
var db *sql.DB

func DatabaseInit() *sql.DB {
	psqlInfo := fmt.Sprintf("host=%s port=%s user=%s "+
		"password=%s dbname=%s",
		os.Getenv("dbHost"), os.Getenv("dbPort"), os.Getenv("dbUser"), os.Getenv("dbPassword"), os.Getenv("dbName"))
	once.Do(func() {
		var err error
		db, err = sql.Open("postgres", psqlInfo)
		if err != nil {
			panic(err)
		}
	})

	return db
}

func InsertJobsIntoPostgresDb(infos *[]*scraping.JobInfo) error {
	DatabaseInit()
	for _, info := range *infos {
		_, err := db.Exec("insert into Jobs(Title, Company, Site, Url, ScrapeDate, Description) VALUES ($1, $2, $3, $4, $5, $6);", info.Title, info.Company, info.Site, info.Url, info.Date, info.Description)
		if err != nil {
			return err
		}
	}
	return nil

}

func GetAllJobs() *[]*scraping.JobInfo {
	DatabaseInit()
	rows, err := db.Query("select Title, Company, Site, Url, ScrapeDate from Jobs")
	if err != nil {
		panic(err)
	}
	var jobs []*scraping.JobInfo

	for rows.Next() {
		job := scraping.JobInfo{}
		if err := rows.Scan(&job.Title, &job.Company, &job.Site, &job.Url, &job.Date); err != nil {
			panic(err)
		}
		jobs = append(jobs, &job)
	}
	return &jobs
}
