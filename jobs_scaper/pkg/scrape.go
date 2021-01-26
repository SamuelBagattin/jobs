package pkg

import (
	"log"
	"strings"
	"time"

	"github.com/gocolly/colly/v2"
)

func ScrapeIndeed(url string) *[]*JobInfo {
	var currentDate = time.Now()

	log.Println("visiting", url)

	c := colly.NewCollector(
		colly.Async(true),
	)

	c.OnRequest(func(request *colly.Request) {
		request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36")
		request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
		request.Headers.Add("Accept-Encoding", "gzip, deflate, br")
		request.Headers.Add("Accept-Language", "en-US,en;q=0.5")
		request.Headers.Add("Cache-Control", "max-age=0")
		request.Headers.Add("Referer", "max-age=0")
		request.Headers.Add("Connection", "keep-alive")
		request.Headers.Add("Upgrade-Insecure-Requests", "1")
	})

	var page []*JobInfo
	// count links
	c.OnHTML("#pageContent #resultsCol .jobsearch-SerpJobCard", func(e *colly.HTMLElement) {
		job := JobInfo{
			Site: "indeed",
			Id: RandStringBytesMaskImprSrcUnsafe(6),
		}
		e.ForEach(".title", func(i int, element *colly.HTMLElement) {
			job.Title = strings.TrimSpace(element.Text)
		})
		e.ForEach(".title > a", func(i int, element *colly.HTMLElement) {
			job.Url = "https://fr.indeed.com" + strings.TrimSpace(element.Attr("href"))
		})
		e.ForEach(".company", func(i int, element *colly.HTMLElement) {
			job.Company = strings.TrimSpace(element.Text)
		})
		job.Date = currentDate
		page = append(page, &job)
	})

	c.OnHTML("#pageContent #resultsCol .pagination [aria-label=\"Suivant\"]", func(element *colly.HTMLElement) {
		time.Sleep(1 * time.Second)
		err := element.Request.Visit("https://fr.indeed.com" + element.Attr("href"))
		if err != nil {
			panic(err)
		}
	})

	c.OnResponse(func(r *colly.Response) {
		log.Println(r.Request.URL)
	})
	c.OnError(func(r *colly.Response, err error) {
		log.Println("error:", r.StatusCode, err)
	})

	err := c.Visit(url)
	if err != nil {
		panic(err)
	}
	c.Wait()

	return &page
}

