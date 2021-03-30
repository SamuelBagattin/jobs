package scraping

import (
	"fmt"
	"github.com/gocolly/colly/v2"
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
			log.Trace(l.logWithName("Visiting next page: " + l.getNextPageUrl(&i)))
			visitError := c.Visit(l.getNextPageUrl(&i))
			if visitError != nil {
				log.Error("Error while visiting: " + l.getNextPageUrl(&i))
				panic(visitError)
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
			req.Headers.Add("cache-control", "max-age=0")
			req.Headers.Add("upgrade-insecure-requests", "1")
			req.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
			req.Headers.Add("sec-gpc", "1")
			req.Headers.Add("sec-fetch-site", "same-site")
			req.Headers.Add("sec-fetch-mode", "navigate")
			req.Headers.Add("sec-fetch-user", "?1")
			req.Headers.Add("sec-fetch-dest", "document")
			req.Headers.Add("referer", "https://www.linkedin.com/")
			req.Headers.Add("accept-language", "en-US,en;q=0.9")
		})

		cDescription.OnError(func(r *colly.Response, err error) {
			log.Warning(l.logWithName("error description:"), r.StatusCode, err, r.Headers)
		})

		cDescription.OnResponse(func(r *colly.Response) {
			log.Println(l.logWithName("response received description"), r.StatusCode)
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
			job.Url = strings.TrimSpace(element.Attr("href"))
			time.Sleep(utils.RandScrapingInterval())
			errDesc := cDescription.Visit(job.Url)
			if errDesc != nil {
				log.Error("Error while visiting description: " + job.Url)
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
	return l.config
}

func (l *LinkedinClient) logWithName(msg string) string {
	return fmt.Sprintf("%s: %s", l.config.SiteName, msg)
}
