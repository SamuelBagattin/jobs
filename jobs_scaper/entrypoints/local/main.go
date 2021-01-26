package main

import (
	"encoding/json"
	"jobs_scaper/pkg"
	"log"
	"net/http"
)

func main() {
	addr := ":7171"

	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		indeedUrl := "https://fr.indeed.com/jobs?q=Developpeur&l=Bordeaux%20(33)&radius=5"
		indeed := pkg.ScrapeIndeed(indeedUrl)
		err := pkg.InsertJobsIntoPostgresDb(indeed)
		if err != nil {
			panic(err)
		}
		b, err := json.Marshal(indeed)
		if err != nil {
			log.Println("failed to serialize response:", err)
			return
		}
		w.Header().Add("Content-Type", "application/json")
		_, responseErr := w.Write(b)
		if responseErr != nil {
			panic(responseErr)
		}
	},
	)

	log.Println("listening on", addr)
	log.Fatal(http.ListenAndServe(addr, nil))

}
