package scraping

import (
	"fmt"
	"github.com/gocolly/colly/v2"
	"github.com/gocolly/colly/v2/extensions"
	log "github.com/sirupsen/logrus"
	"jobs_scaper/pkg/utils"
	"net/url"
	"strings"
	"time"
)

type IndeedClient struct {
	config *ClientConfig
}

func NewIndeedClient() (*IndeedClient, error) {
	log.Trace("Initiating indeed client")
	var baseUrl, err = url.Parse("https://fr.indeed.com")
	if err != nil {
		return nil, err
	}
	return &IndeedClient{
		config: &ClientConfig{
			SiteName:     "indeed",
			SiteBasePath: baseUrl,
			SiteOrigin:   baseUrl.Host,
		},
	}, nil
}

func (l IndeedClient) GetConfig() *ClientConfig {
	var conf = *l.config
	return &conf
}

func (l *IndeedClient) logWithName(msg string) string {
	return fmt.Sprintf("%s: %s", l.config.SiteName, msg)
}

func (l IndeedClient) Scrape(query string) (*[]*JobInfo, error) {
	var currentDate = time.Now()
	var urll = "https://fr.indeed.com/emplois?q=" + url.QueryEscape(query) + "&l=bordeaux"
	log.Println("visiting", urll)

	c := colly.NewCollector(
		colly.AllowURLRevisit(),
	)

	extensions.RandomMobileUserAgent(c)

	c.OnRequest(func(request *colly.Request) {
		request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8")
		request.Headers.Add("Accept-Language", "en-US,en;q=0.5")
		request.Headers.Add("Referer", "https://fr.indeed.com/Bordeaux-(33)-Emplois")
		request.Headers.Add("Connection", "keep-alive")
		request.Headers.Add("Cookie", "CTK=1et58d6gsss0o800; RF=\"TFTzyBUJoNr6YttPP3kyivpZ6-9J49o-Uk3iY6QNQqKE2fh7FyVgtZtPsJflJ0UrAZ9dg-fttnA=\"; indeed_rcc=\"PREF:LV:CTK:UD:RQ\"; LV=\"LA=1611863071:CV=1611863071:TS=1611863071\"; UD=\"LA=1617304351:LV=1617295250:CV=1617304351:TS=1611863077:SG=4edad7b3a8a58dfc844c851dc46bf4d1\"; CTK=1et58d6gsss0o800; OptanonConsent=isIABGlobal=false&datestamp=Thu+Apr+01+2021+21%3A12%3A35+GMT%2B0200+(heure+d%E2%80%99%C3%A9t%C3%A9+d%E2%80%99Europe+centrale)&version=6.13.0&hosts=&consentId=d04ffe85-3684-4051-bd4c-bbfb0b9f582e&interactionCount=2&landingPath=NotLandingPage&groups=C0001%3A1%2CC0002%3A1%2CC0003%3A1%2CC0004%3A1&AwaitingReconsent=false&geolocation=%3B; PREF=\"TM=1617304351380:L=Bordeaux+%2833%29\"; RQ=\"q=&l=Bordeaux+%2833%29&ts=1617304351395:q=Informatique&l=&ts=1617295250267:q=developpeur&l=bordeaux&ts=1611863092663\"; pjps=1; OptanonAlertBoxClosed=2021-01-29T20:01:04.839Z; jasxMarvin=1; loctip=1; INDEED_CSRF_TOKEN=1kUcKT62Jds2MEx44bQPwbTZyHdL09D9; JSESSIONID=6A7271F60A3EA220AD246E2B26CAB6A8; jaSerpCount=4; PPN=1; PTK=\"tk=1f27djskk3jjn001&type=jobsearch&subtype=topsearch\"")
		request.Headers.Add("Upgrade-Insecure-Requests", "1")
		request.Headers.Add("TE", "Trailers")
	})

	var page []*JobInfo
	jobCounter := 0
	// count links
	c.OnHTML("#pageContent #resultsCol .jobsearch-SerpJobCard", func(e *colly.HTMLElement) {
		cDescription := c.Clone()
		extensions.RandomMobileUserAgent(cDescription)

		cDescription.OnRequest(func(req *colly.Request) {
			req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8")
			req.Headers.Add("Accept-Language", "en-US,en;q=0.5")
			req.Headers.Add("Connection", "keep-alive")
			req.Headers.Add("Cookie", "CTK=1et58d6gsss0o800; RF=\"TFTzyBUJoNr6YttPP3kyivpZ6-9J49o-Uk3iY6QNQqKE2fh7FyVgtZtPsJflJ0UrAZ9dg-fttnA=\"; indeed_rcc=\"PREF:LV:CTK:UD:RQ\"; LV=\"LA=1611863071:CV=1611863071:TS=1611863071\"; UD=\"LA=1617304365:LV=1617295250:CV=1617304351:TS=1611863077:SG=467470ea46e8d37f70d76fca9a6e9692\"; CTK=1et58d6gsss0o800; OptanonConsent=isIABGlobal=false&datestamp=Thu+Apr+01+2021+21%3A12%3A51+GMT%2B0200+(heure+d%E2%80%99%C3%A9t%C3%A9+d%E2%80%99Europe+centrale)&version=6.13.0&hosts=&consentId=d04ffe85-3684-4051-bd4c-bbfb0b9f582e&interactionCount=2&landingPath=NotLandingPage&groups=C0001%3A1%2CC0002%3A1%2CC0003%3A1%2CC0004%3A1&AwaitingReconsent=false&geolocation=%3B; PREF=\"TM=1617304365028:L=bordeaux\"; RQ=\"q=developpeur&l=bordeaux&ts=1617304365042&pts=1611863092663:q=&l=Bordeaux+%2833%29&ts=1617304351395:q=Informatique&l=&ts=1617295250267\"; pjps=1; OptanonAlertBoxClosed=2021-01-29T20:01:04.839Z; jasxMarvin=1; loctip=1; INDEED_CSRF_TOKEN=1kUcKT62Jds2MEx44bQPwbTZyHdL09D9; JSESSIONID=474D10954D79CD0C2E7F10B23D9FDF91; jaSerpCount=5; PPN=1; RSJC=d9fdcd6009b0b65f; LC=\"co=FR&hl=fr_FR\"; CLK=d9fdcd6009b0b65f; VJP=\"jk=d9fdcd6009b0b65f&q=developpeur&l=bordeaux&tk=1f27dk9v4strb800&from=web&advn=586662583807989\"; pub=4a1b367933fd867b19b072952f68dceb")
			req.Headers.Add("Upgrade-Insecure-Requests", "1")
			req.Headers.Add("TE", "Trailers")
		})

		job := JobInfo{
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
			job.Url = l.config.SiteBasePath.String() + strings.TrimSpace(element.Attr("href"))
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
		err := element.Request.Visit(l.config.SiteBasePath.String() + element.Attr("href"))
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

	err := c.Visit(urll)
	if err != nil {
		panic(err)
	}
	c.Wait()

	return &page, nil
}
