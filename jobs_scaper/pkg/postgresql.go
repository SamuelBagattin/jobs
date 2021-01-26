package pkg

import (
	"database/sql"
	"fmt"
	_ "github.com/lib/pq"
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

func InsertJobsIntoPostgresDb(infos *[]*JobInfo) error  {
	DatabaseInit()
	for _, info := range *infos {
		_, err := db.Exec("insert into Jobs(Title, Company, Site, Url, ScrapeDate) VALUES ($1, $2, $3, $4, $5);", info.Title, info.Company, info.Site, info.Url, info.Date)
		if err != nil {
			return err
		}
	}
	return nil

}
