package scraping

import (
	"fmt"
	"github.com/gocolly/colly/v2"
	"jobs_scaper/pkg/utils"
	"log"
	"net/url"
	"strconv"
	"strings"
	"time"
)

func NewLinkedinClient() (*LinkedinClient, error) {
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
	var i = 0
	//var visitUrl = fmt.Sprintf("%s%s",linkedinBaseUrl, linkedinScrapePath) + "?keywords=developpeur&location=Bordeaux%2C%20Nouvelle-Aquitaine%2C%20France&geoId=&trk=homepage-jobseeker_jobs-search-bar_search-submit&redirect=false&position=0&pageNum=0&start=0"
	var visitUrl = l.getNextPageUrl(&i)
	c := colly.NewCollector(
		//colly.Async(true),
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
		req.Headers.Add("Cookie", "bcookie=\"v=2&c25ca2ec-d74a-457e-8f13-9f95818fe99d\"; bscookie=\"v=1&20210131180058d9865b53-c3af-4982-8850-7aec07a7167eAQEfCpoO4Ywrbz6jCtYTzFcCpZNm2hDN\"; li_gc=MTswOzE2MTIxMTYwNTg7MjswMjHjqA+pB1u/nH/vOO6UPdoUfUBReBUC/K1vo0rXI/5jCA==; JSESSIONID=ajax:4373026180247572192; lang=v=2&lang=en-us; lidc=\"b=TGST01:s=T:r=T:g=2430:u=1:i=1612691196:t=1612777596:v=1:sig=AQHfPJf0IY-n7JJbRORsXUKix8tw2Pf3\"")
	})

	var page []*JobInfo

	// extract status code
	c.OnResponse(func(r *colly.Response) {
		log.Println("response received", r.StatusCode)
	})
	c.OnScraped(func(response *colly.Response) {
		i = len(page)
		fmt.Printf("%v", i)
		if i <= 500 {
			visitError := c.Visit(l.getNextPageUrl(&i))
			if visitError != nil {
				panic(visitError)
			}
		}
	})
	c.OnError(func(r *colly.Response, err error) {
		log.Println("error:", r.StatusCode, err, r.Headers)
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
			req.Headers.Add("cookie", "JSESSIONID=ajax:2304572925030108609; bcookie=\"v=2&13e101e6-0b38-46ee-8c1e-ec65b494dcdf\"; bscookie=\"v=1&2020113017202465f08274-f81a-4ebd-838b-52f3aafad8dcAQEL0Q_H8Wrk4LCYX3aCoYo17DyEbisG\"; lissc=1; G_ENABLED_IDPS=google; g_state={\"i_p\":1606764028010,\"i_l\":1}; AMCVS_14215E3D5995C57C0A495C55%40AdobeOrg=1; li_gc=MTsyMTsxNjEwOTEyMDYyOzI7MDIxcAKuLSJg/Wi5pOwlrFA8NwnR73J+Z0J8hd+6hh2Wntk=; _gcl_au=1.1.1196465340.1610912093; li_mc=MTsyMTsxNjEwOTEyNjQ4OzI7MDIxrXQ9bAU9po6qOJiexUiIE0qAs0nL1AcxG0Ek5kpTpns=; lang=v=2&lang=en-us; _ga=GA1.2.1490786771.1610912703; aam_uuid=34701201306978210768032937137846489633; lidc=\"b=OGST01:s=O:r=O:g=2295:u=1:i=1612710576:t=1612796976:v=1:sig=AQGeMBi1PDBEoMiz1tzmqdUuiIAw9hn4\"; AMCV_14215E3D5995C57C0A495C55%40AdobeOrg=-637568504%7CMCIDTS%7C18666%7CMCMID%7C35239181505128059838049505903047664106%7CMCOPTOUT-1612720866s%7CNONE%7CvVersion%7C5.1.1%7CMCAAMLH-1611517503%7C6%7CMCAAMB-1612698514%7Cj8Odv6LonN4r3an7LhD3WZrU1bUpAkFkkiY1ncBR96t2PTI; recent_history=AQHsFJHBP-Wg3gAAAXd9OgdcTYyQdvDQdUwDvsrbfVoHxZJO5AEYgMEUkP-DT9MyxRt7RyFnRLHRAVtEbounv6RDdNjjCQ2pyAjj2GmorG_QMugGv-EtX5ptMtCVm0Un-FzHADLMgIZN_MaB8RSvNzw2EDIs153d5Y-LJK6uCPnioMwCywgdAOxIUx912jvoJUeWiA-u6mpK31wVqvlyBERL-SqnmLepBTOZ21o-6JnOdYli9ed5RDitpYwsn4iYA8uGpIWlxUQt9GfPhwqqs1MdgDFEalN5j7WE3BbF44AWr9T22s2_ecS201vmkmTu-movOWxQckcp6vyIqX7Rw4p4XWn_q3pFGbwR6bE5M7hObR1AFVAqz9oErkt_TUr2RiRGs2QtvPV_gfE1wAvPIQ; bcookie=\"v=2&c25ca2ec-d74a-457e-8f13-9f95818fe99d\"; li_gc=MTswOzE2MTIxMTYwNTg7MjswMjHjqA+pB1u/nH/vOO6UPdoUfUBReBUC/K1vo0rXI/5jCA==; lang=v=2&lang=en-us; lidc=\"b=TGST01:s=T:r=T:g=2430:u=1:i=1612691196:t=1612777596:v=1:sig=AQHfPJf0IY-n7JJbRORsXUKix8tw2Pf3\"")
		})

		cDescription.OnError(func(r *colly.Response, err error) {
			log.Println("error description:", r.StatusCode, err, r.Headers)
		})

		cDescription.OnResponse(func(r *colly.Response) {
			log.Println("response received description", r.StatusCode)
		})

		job := JobInfo{
			Site: "linkedin",
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
				panic(errDesc)
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
