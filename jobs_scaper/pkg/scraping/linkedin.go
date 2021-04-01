package scraping

import (
	"fmt"
	"github.com/gocolly/colly/v2"
	"github.com/gocolly/colly/v2/extensions"
	log "github.com/sirupsen/logrus"
	"jobs_scaper/pkg/utils"
	"net/url"
	"strconv"
	"strings"
	"time"
)

func NewLinkedinClient() (*LinkedinClient, error) {
	log.Trace("Initiating linkedin client")
	var baseUrl, err = url.Parse("https://fr.linkedin.com")
	if err != nil {
		return nil, err
	}
	return &LinkedinClient{
		config: &ClientConfig{
			SiteName:     "linkedin",
			SiteBasePath: baseUrl,
			SiteOrigin:   baseUrl.Host,
		},
	}, nil
}

type LinkedinClient struct {
	config     *ClientConfig
	maxResults *int
}

func (l *LinkedinClient) Scrape() (*[]*JobInfo, error) {
	log.Trace(l.logWithName("Start scraping"))
	var i = 0
	var visitUrl = l.getNextPageUrl(&i)
	c := colly.NewCollector(
		colly.UserAgent("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.101 Safari/537.36"),
	)

	c.OnRequest(func(req *colly.Request) {
		req.Headers.Add("authority", "www.linkedin.com")
		req.Headers.Add("accept", "*/*")
		req.Headers.Add("sec-gpc", "1")
		req.Headers.Add("sec-fetch-site", "same-origin")
		req.Headers.Add("sec-fetch-mode", "cors")
		req.Headers.Add("sec-fetch-dest", "empty")
		req.Headers.Add("referer", "https://www.linkedin.com/jobs/search?keywords=developpeur&location=Bordeaux%2C%20Nouvelle-Aquitaine%2C%20France&geoId=&trk=homepage-jobseeker_jobs-search-bar_search-submit&redirect=false&position=1&pageNum=0")
		req.Headers.Add("accept-language", "en-US,en;q=0.9")
	})

	var page []*JobInfo

	c.OnScraped(func(response *colly.Response) {
		i = len(page)
		log.Debug(l.logWithName(fmt.Sprintf("%v", i)))
		if i <= 300 {
			retryErr := utils.ExecuteWithRetries(func() error {
				log.Trace(l.logWithName("Visiting next page: " + l.getNextPageUrl(&i)))
				return c.Visit(l.getNextPageUrl(&i))
			}, 3)
			if retryErr != nil {
				panic(retryErr)
			}
		}
	})
	c.OnError(func(r *colly.Response, err error) {
		log.Println(l.logWithName("Error while visiting: "), r.Request.URL.String(), r.StatusCode, err, string(r.Body))
	})
	c.OnHTML(".result-card.job-result-card.result-card--with-hover-state", func(element *colly.HTMLElement) {
		cDescription := c.Clone()
		cDescription.OnRequest(func(req *colly.Request) {
			req.Headers.Add("authority", "fr.linkedin.com")
			req.Headers.Add("pragma", "no-cache")
			req.Headers.Add("cache-control", "no-cache")
			req.Headers.Add("upgrade-insecure-requests", "1")
			//req.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.105 Safari/537.36")
			req.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
			req.Headers.Add("sec-gpc", "1")
			req.Headers.Add("sec-fetch-site", "none")
			req.Headers.Add("sec-fetch-mode", "navigate")
			req.Headers.Add("sec-fetch-user", "?1")
			req.Headers.Add("sec-fetch-dest", "document")
			req.Headers.Add("accept-language", "fr-CH, fr;q=0.9, en;q=0.8, de;q=0.7, *;q=0.5")
		})

		cDescription.OnError(func(r *colly.Response, err error) {
			log.WithFields(log.Fields{
				"statusCode": r.StatusCode,
			}).Warning(l.logWithName("Error while fetching description"), r.StatusCode, err)
		})

		job := JobInfo{
			Site: l.config.SiteName,
			Id:   utils.RandStringBytesMaskImprSrcUnsafe(6),
			Date: time.Now(),
		}

		cDescription.OnHTML(".show-more-less-html__markup", func(desc *colly.HTMLElement) {
			job.Description = strings.TrimSpace(desc.Text)
		})

		element.ForEach("a.result-card__full-card-link", func(i int, element *colly.HTMLElement) {
			var fullUrl = strings.TrimSpace(element.Attr("href"))
			job.Url = sanitizeUrl(fullUrl)
			time.Sleep(utils.RandScrapingInterval())
			err := utils.ExecuteWithRetries(func() error {
				log.Trace(l.logWithName("Visiting Description: " + fullUrl))
				extensions.RandomUserAgent(c)
				return cDescription.Visit(fullUrl)
			}, 3)
			if err != nil {
				panic(err)
			}
		})
		element.ForEach(".screen-reader-text", func(i int, element *colly.HTMLElement) {
			job.Title = strings.TrimSpace(element.Text)
		})
		element.ForEach("h4", func(i int, element *colly.HTMLElement) {
			job.Company = strings.TrimSpace(element.Text)
		})
		page = append(page, &job)
	})

	err := c.Visit(visitUrl)
	if err != nil {
		panic(err)
	}
	c.Wait()
	return &page, nil

}

func (l *LinkedinClient) getNextPageUrl(resultsCount *int) string {
	return "https://fr.linkedin.com/jobs-guest/jobs/api/seeMoreJobPostings/search?keywords=developpeur&location=Bordeaux%2C%20Nouvelle-Aquitaine%2C%20France&geoId=&trk=homepage-jobseeker_jobs-search-bar_search-submit&redirect=false&position=0&pageNum=0&start=" + strconv.Itoa(*resultsCount)
}

func (l LinkedinClient) GetConfig() *ClientConfig {
	var conf = *l.config
	return &conf
}

func (l *LinkedinClient) logWithName(msg string) string {
	return fmt.Sprintf("%s: %s", l.config.SiteName, msg)
}

func sanitizeUrl(urlString string) string {
	parsedUrl, err := url.Parse(urlString)
	if err != nil {
		panic(err)
	}
	parsedUrl.RawQuery = ""
	return parsedUrl.String()
}
