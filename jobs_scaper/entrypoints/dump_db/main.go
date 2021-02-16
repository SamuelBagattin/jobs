package main

import (
	"encoding/json"
	"jobs_scaper/pkg"
	"log"
	"os"
)

func main() {
	result, err := json.Marshal(pkg.GetAllJobs())
	if err != nil {
		panic(err)
	}
	f, err := os.Create("dump.json")

	if err != nil {
		log.Fatal(err)
	}

	defer func() {
		closeErr := f.Close()
		if closeErr != nil {
			panic(closeErr)
		}
	}()

	_, err2 := f.Write(result)

	if err2 != nil {
		log.Fatal(err2)
	}
}
