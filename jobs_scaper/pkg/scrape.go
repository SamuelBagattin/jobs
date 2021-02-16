package pkg

import (
	"github.com/gocolly/colly/v2/extensions"
	"jobs_scaper/pkg/scraping"
	"jobs_scaper/pkg/utils"
	"log"
	"strings"
	"time"

	"github.com/gocolly/colly/v2"
)

// Deprecated cuz indeed keeps blocking me
func ScrapeIndeed(url string) *[]*scraping.JobInfo {
	const proxyDomain = "https://jtkx88oisl.execute-api.us-east-1.amazonaws.com/prod"

	var currentDate = time.Now()

	log.Println("visiting", url)

	c := colly.NewCollector(
		colly.AllowURLRevisit(),
	)

	extensions.RandomMobileUserAgent(c)

	if limitErr := c.Limit(&colly.LimitRule{
		DomainRegexp: ".*",
		Delay:        time.Second,
		RandomDelay:  time.Second,
		Parallelism:  2,
	}); limitErr != nil {
		panic(limitErr)
	}

	c.OnRequest(func(request *colly.Request) {
		request.Headers.Add("Accept", "text/html")
		request.Headers.Add("Accept-Encoding", "gzip, deflate, br")
		request.Headers.Add("Accept-Language", "en-US,en;q=0.5")
		request.Headers.Add("Cache-Control", "max-age=0")
		request.Headers.Add("Referer", "max-age=0")
		request.Headers.Add("Connection", "keep-alive")
		request.Headers.Add("Upgrade-Insecure-Requests", "1")
		request.Headers.Add("authority", "fr.indeed.com")
		request.Headers.Add("referer", "https://fr.indeed.com/")
	})

	var page []*scraping.JobInfo
	jobCounter := 0
	// count links
	c.OnHTML("#pageContent #resultsCol .jobsearch-SerpJobCard", func(e *colly.HTMLElement) {
		cDescription := c.Clone()
		extensions.RandomMobileUserAgent(cDescription)

		if limitErr := cDescription.Limit(&colly.LimitRule{
			DomainRegexp: ".*",
			Delay:        time.Second,
			RandomDelay:  time.Second,
			Parallelism:  2,
		}); limitErr != nil {
			panic(limitErr)
		}

		cDescription.OnRequest(func(req *colly.Request) {
			req.Headers.Add("authority", "fr.indeed.com")
			req.Headers.Add("cache-control", "max-age=0")
			req.Headers.Add("upgrade-insecure-requests", "1")
			req.Headers.Add("user-agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.101 Safari/537.36")
			req.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
			req.Headers.Add("sec-gpc", "1")
			req.Headers.Add("sec-fetch-site", "same-origin")
			req.Headers.Add("sec-fetch-mode", "navigate")
			req.Headers.Add("sec-fetch-user", "?1")
			req.Headers.Add("sec-fetch-dest", "document")
			req.Headers.Add("referer", "https://fr.indeed.com/")
			req.Headers.Add("accept-language", "en-US,en;q=0.9")
		})

		job := scraping.JobInfo{
			Site: "indeed",
			Id:   utils.RandStringBytesMaskImprSrcUnsafe(6),
			Date: time.Now(),
		}

		cDescription.OnHTML(".jobsearch-JobComponent-description", func(desc *colly.HTMLElement) {
			job.Description = strings.TrimSpace(desc.Text)
		})

		e.ForEach(".title", func(i int, element *colly.HTMLElement) {
			job.Title = strings.TrimSpace(element.Text)
		})
		e.ForEach(".title > a", func(i int, element *colly.HTMLElement) {
			jobCounter++
			log.Println(jobCounter)
			job.Url = proxyDomain + strings.TrimSpace(element.Attr("href"))
			errDesc := cDescription.Visit(job.Url)
			if errDesc != nil {
				panic(errDesc)
			}
		})
		e.ForEach(".company", func(i int, element *colly.HTMLElement) {
			job.Company = strings.TrimSpace(element.Text)
		})
		job.Date = currentDate
		page = append(page, &job)
	})

	c.OnHTML("#pageContent #resultsCol .pagination [aria-label=\"Suivant\"]", func(element *colly.HTMLElement) {
		err := element.Request.Visit(proxyDomain + element.Attr("href"))
		if err != nil {
			panic(err)
		}
	})

	c.OnResponse(func(r *colly.Response) {
		log.Println(string(r.Body))
		log.Println(r.Request.URL)
	})
	c.OnError(func(r *colly.Response, err error) {
		log.Println(string(r.Body))
		log.Println("error:", r.StatusCode, err)
	})

	err := c.Visit(url)
	if err != nil {
		panic(err)
	}
	c.Wait()

	return &page
}
