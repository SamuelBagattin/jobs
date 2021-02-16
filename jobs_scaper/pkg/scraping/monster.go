package scraping

import (
	"fmt"
	"github.com/gocolly/colly/v2"
	"jobs_scaper/pkg/utils"
	"log"
	"net/url"
	"strings"
	"time"
)

type MonsterClient struct {
	config     *ClientConfig
	maxResults *int
}

func NewMonsterClient() (*MonsterClient, error) {
	var baseUrl, err = url.Parse("https://www.monster.fr")
	if err != nil {
		return nil, err
	}
	return &MonsterClient{
		config: &ClientConfig{
			SiteName:     "monster",
			SiteBasePath: baseUrl,
			SiteOrigin:   baseUrl.Host,
		},
	}, nil
}

func (m *MonsterClient) GetConfig() *ClientConfig {
	return m.config
}

func (m *MonsterClient) Scrape() (*[]*JobInfo, error) {
	var descCounter = 0
	var visitUrl = m.config.SiteBasePath.String() + "/emploi/recherche/?q=d__C3__A9veloppeur&where=bordeaux&cy=fr&stpage=10&page=10"

	c := colly.NewCollector(
		//colly.Async(true),
		colly.AllowURLRevisit(),
		colly.UserAgent("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.101 Safari/537.36"),
	)

	c.OnRequest(func(req *colly.Request) {
		req.Headers.Add("Connection", "keep-alive")
		req.Headers.Add("Cache-Control", "max-age=0")
		req.Headers.Add("Upgrade-Insecure-Requests", "1")
		req.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.101 Safari/537.36")
		req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
		req.Headers.Add("Sec-GPC", "1")
		req.Headers.Add("Sec-Fetch-Site", "same-origin")
		req.Headers.Add("Sec-Fetch-Mode", "navigate")
		req.Headers.Add("Sec-Fetch-User", "?1")
		req.Headers.Add("Sec-Fetch-Dest", "document")
		req.Headers.Add("Referer", "https://www.monster.fr/emploi/recherche?q=d%c3%a9veloppeur&,,=&cy=fr&rad=20")
		req.Headers.Add("Accept-Language", "en-US,en;q=0.9")
	})

	var page []*JobInfo

	// extract status code
	c.OnResponse(func(r *colly.Response) {
		//fmt.Println(string(r.Body))
		log.Println("response received", r.StatusCode)
	})
	c.OnRequest(func(request *colly.Request) {
		log.Println("visiting", request.URL)
	})
	c.OnError(func(r *colly.Response, err error) {
		log.Println("error:", r.StatusCode, err, r.Headers)
	})
	c.OnHTML("#SearchResults>.card-content[data-jobid]", func(element *colly.HTMLElement) {
		cDescription := c.Clone()
		cDescription.OnRequest(func(req *colly.Request) {
			req.Headers.Add("Connection", "keep-alive")
			req.Headers.Add("Upgrade-Insecure-Requests", "1")
			req.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.101 Safari/537.36")
			req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
			req.Headers.Add("Sec-GPC", "1")
			req.Headers.Add("Sec-Fetch-Site", "same-site")
			req.Headers.Add("Sec-Fetch-Mode", "navigate")
			req.Headers.Add("Sec-Fetch-User", "?1")
			req.Headers.Add("Sec-Fetch-Dest", "document")
			req.Headers.Add("Referer", "https://www.monster.fr/")
			req.Headers.Add("Accept-Language", "en-US,en;q=0.9")
		})

		cDescription.OnError(func(r *colly.Response, err error) {
			log.Println("error description:", r.StatusCode, err, r.Headers)
		})

		cDescription.OnResponse(func(r *colly.Response) {
			log.Println("response received description", r.StatusCode)
		})

		job := JobInfo{
			Site: "monster",
			Id:   utils.RandStringBytesMaskImprSrcUnsafe(6),
			Date: time.Now(),
		}

		cDescription.OnHTML("#JobDescription", func(desc *colly.HTMLElement) {
			descCounter++
			log.Println(descCounter)
			job.Description = strings.TrimSpace(desc.Text)
		})

		element.ForEach(".title>a", func(i int, element *colly.HTMLElement) {
			job.Url = strings.TrimSpace(element.Attr("href"))
			job.Title = strings.TrimSpace(element.Text)
			//time.Sleep(RandScrapingInterval())
			errDesc := cDescription.Visit(job.Url)
			if errDesc != nil {
				fmt.Println("Error :", errDesc)
			}
		})
		element.ForEach(".company>.name", func(i int, element *colly.HTMLElement) {
			job.Company = strings.TrimSpace(element.Text)
		})
		page = append(page, &job)
	})

	err := c.Visit(visitUrl)
	if err != nil {
		return nil, err
	}
	c.Wait()
	return &page, nil

}
