package scraping

import (
	"encoding/json"
	"fmt"
	"github.com/aws/aws-sdk-go/aws"
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
		maxResults: aws.Int(200),
	}, nil
}

type LinkedinClient struct {
	config     *ClientConfig
	maxResults *int
}

func (l *LinkedinClient) Scrape(query string) (*[]*JobInfo, error) {
	log.Trace(l.logWithName("Start scraping"))
	var i = 0
	var visitUrl = l.getNextPageUrl(&i, query)
	c := colly.NewCollector()
	extensions.RandomUserAgent(c)

	c.OnRequest(func(req *colly.Request) {
		req.Headers.Add("authority", "fr.linkedin.com")
		req.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"93\", \" Not;A Brand\";v=\"99\", \"Chromium\";v=\"93\"")
		req.Headers.Add("csrf-token", "ajax:7709767641608695741")
		req.Headers.Add("sec-ch-ua-mobile", "?0")
		req.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36")
		req.Headers.Add("sec-ch-ua-platform", "\"Windows\"")
		req.Headers.Add("accept", "*/*")
		req.Headers.Add("sec-fetch-site", "same-origin")
		req.Headers.Add("sec-fetch-mode", "cors")
		req.Headers.Add("sec-fetch-dest", "empty")
		req.Headers.Add("referer", "https://fr.linkedin.com/jobs/search?keywords=d%C3%A9veloppeur&location=Bordeaux%2C%20Nouvelle-Aquitaine%2C%20France&geoId=&trk=homepage-jobseeker_jobs-search-bar_search-submit&position=1&pageNum=0")
		req.Headers.Add("accept-language", "en-US,en;q=0.9")
	})

	var page []*JobInfo
	var cookie = ""

	c.OnResponseHeaders(func(response *colly.Response) {
		cookie = response.Headers.Get("set-cookie")
	})

	c.OnScraped(func(response *colly.Response) {
		i = len(page)
		log.Debug(l.logWithName(fmt.Sprintf("%v", i)))
		if i <= *l.maxResults {
			retryErr := utils.ExecuteWithRetries(func() error {
				log.Trace(l.logWithName("Visiting next page: " + l.getNextPageUrl(&i, query)))
				l.logWithName("visiting: " + l.getNextPageUrl(&i, query))
				visitError := c.Visit(l.getNextPageUrl(&i, query))
				return visitError
			}, 3)
			if retryErr != nil {
				log.Warning(l.logWithName("Error while fetching next page"))
				err := utils.WriteToFile("linkedin.html", string(response.Body))
				if err != nil {
					panic(err)
				}
				headers, headersErr := json.Marshal(*response.Headers)
				if headersErr != nil {
					panic(headersErr)
				}
				err = utils.WriteToFile("headers.json", string(headers))
				if err != nil {
					panic(err)
				}
			}
		}
	})
	c.OnError(func(r *colly.Response, err error) {
		log.Println(l.logWithName("Error while visiting: "), r.Request.URL.String(), r.StatusCode, err, string(r.Body))
	})
	c.OnHTML(".job-search-card", func(element *colly.HTMLElement) {
		job := JobInfo{
			Site: l.config.SiteName,
			Id:   utils.RandStringBytesMaskImprSrcUnsafe(6),
			Date: time.Now(),
		}

		cDescription := colly.NewCollector()
		extensions.RandomUserAgent(cDescription)
		cDescription.OnRequest(func(req *colly.Request) {
			req.Headers.Add("authority", "fr.linkedin.com")
			req.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"93\", \" Not;A Brand\";v=\"99\", \"Chromium\";v=\"93\"")
			req.Headers.Add("sec-ch-ua-mobile", "?0")
			req.Headers.Add("sec-ch-ua-platform", "\"Windows\"")
			req.Headers.Add("upgrade-insecure-requests", "1")
			req.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36")
			req.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
			req.Headers.Add("sec-fetch-site", "same-origin")
			req.Headers.Add("sec-fetch-mode", "navigate")
			req.Headers.Add("sec-fetch-user", "?1")
			req.Headers.Add("sec-fetch-dest", "document")
			req.Headers.Add("referer", "https://fr.linkedin.com/jobs/search?keywords=d%C3%A9veloppeur&location=Bordeaux%2C%20Nouvelle-Aquitaine%2C%20France&geoId=&trk=homepage-jobseeker_jobs-search-bar_search-submit&position=1&pageNum=0")
			req.Headers.Add("accept-language", "en-US,en;q=0.9")
		})

		cDescription.OnError(func(r *colly.Response, err error) {
			log.WithFields(log.Fields{
				"statusCode": r.StatusCode,
			}).Warning(l.logWithName("Error while fetching description"), r.StatusCode, err)
		})
		cDescription.OnHTML(".show-more-less-html__markup", func(desc *colly.HTMLElement) {
			job.Description = strings.TrimSpace(desc.Text)
		})

		// Description + joburl
		element.ForEach(".base-card__full-link,a.base-search-card--link", func(i int, element *colly.HTMLElement) {
			var fullUrl = strings.TrimSpace(element.Attr("href"))
			time.Sleep(utils.RandScrapingInterval())
			err := utils.ExecuteWithRetries(func() error {
				log.Trace(l.logWithName("Visiting Description: " + fullUrl))
				extensions.RandomUserAgent(c)
				return cDescription.Visit(fullUrl)
			}, 3)
			if err != nil {
				log.Warn(l.logWithName(err.Error()))
			}
		})

		// Job URL
		element.ForEach(".base-card__full-link,a.base-search-card--link", func(i int, element *colly.HTMLElement) {
			var fullUrl = strings.TrimSpace(element.Attr("href"))
			job.Url = sanitizeUrl(fullUrl)
		})

		// Title
		element.ForEach(".base-search-card__title", func(i int, element *colly.HTMLElement) {
			job.Title = strings.TrimSpace(element.Text)
		})

		// Company name
		element.ForEach("h4", func(i int, element *colly.HTMLElement) {
			job.Company = strings.TrimSpace(element.Text)
		})
		page = append(page, &job)
	})

	l.logWithName("visiting: " + visitUrl)
	err := c.Visit(visitUrl)
	if err != nil {
		l.logWithName(err.Error())
	}
	c.Wait()
	return &page, nil

}

func (l *LinkedinClient) getNextPageUrl(resultsCount *int, query string) string {

	return "https://fr.linkedin.com/jobs-guest/jobs/api/seeMoreJobPostings/search?keywords=" + url.QueryEscape(query) + "&location=Gironde%2C%20Nouvelle-Aquitaine%2C%20France&geoId=&trk=homepage-jobseeker_jobs-search-bar_search-submit&position=1&pageNum=0&start=" + strconv.Itoa(*resultsCount)
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
