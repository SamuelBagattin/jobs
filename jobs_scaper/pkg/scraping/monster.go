package scraping

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/aws/aws-sdk-go/aws"
	log "github.com/sirupsen/logrus"
	"io/ioutil"
	"jobs_scaper/pkg/utils"
	"net/http"
	"net/url"
	"time"
)

type MonsterClient struct {
	config     *ClientConfig
	maxResults *int
	apiConfig  struct {
		searchJobsUrl *string
	}
}

type monsterApiSearchJobsRequest struct {
	JobQuery monsterApiSearchJobsJobQuery `json:"jobQuery"`
	Offset   int                          `json:"offset"`
	PageSize int                          `json:"pageSize"`
}

type monsterApiSearchJobsJobQuery struct {
	Locations []monsterApiSearchJobsLocation `json:"locations"`
	Query     string                         `json:"query"`
}

type monsterApiSearchJobsLocation struct {
	Address string `json:"address"`
	Country string `json:"country"`
}

type monsterApiSearchJobsResponse struct {
	TotalSize          int `json:"totalSize"`
	EstimatedTotalSize int `json:"estimatedTotalSize"`
	JobResults         []struct {
		JobPosting struct {
			Description        string `json:"description"`
			Url                string `json:"url"`
			Title              string `json:"title"`
			HiringOrganization struct {
				Name string `json:"name"`
			} `json:"hiringOrganization"`
		} `json:"jobPosting"`
	} `json:"jobResults"`
}

func NewMonsterClient() (*MonsterClient, error) {
	log.Trace("Initiating monster client")
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
		apiConfig: struct {
			searchJobsUrl *string
		}{
			searchJobsUrl: aws.String("https://services.monster.io/jobs-svx-service/v2/monster/search-jobs/samsearch/fr-fr"),
		},
	}, nil
}

func (m *MonsterClient) GetConfig() *ClientConfig {
	return m.config
}

func (m *MonsterClient) Scrape(query string) (*[]*JobInfo, error) {
	var scrapedResults []*JobInfo

	request := monsterApiSearchJobsRequest{
		JobQuery: monsterApiSearchJobsJobQuery{
			Locations: []monsterApiSearchJobsLocation{
				{Address: "bordeaux", Country: "fr"},
			},
			Query: query,
		},
		Offset:   0,
		PageSize: 100,
	}

	//log.Trace(m.logWithName("Requesting jobs, offset :" + strconv.Itoa(request.Offset)))
	res, err := m.searchJobs(&request)
	if err != nil {
		log.WithField("request", request).Error("Error while searching jobs")
		panic(err)
	}
	for {
		for _, el := range res.JobResults {
			scrapedResults = append(scrapedResults, &JobInfo{
				Title:       el.JobPosting.Title,
				Company:     el.JobPosting.HiringOrganization.Name,
				Site:        m.config.SiteName,
				Url:         el.JobPosting.Url,
				Date:        time.Now(),
				Id:          utils.RandStringBytesMaskImprSrcUnsafe(6),
				Description: el.JobPosting.Description,
			})
		}
		if res.EstimatedTotalSize <= len(scrapedResults) || len(scrapedResults) > 500 {
			break
		}
		request.Offset = len(scrapedResults)
		//log.Trace(m.logWithName("Requesting jobs, offset :") + strconv.Itoa(request.Offset))
		res2, err2 := m.searchJobs(&request)
		res = res2
		if err2 != nil {
			log.WithField("request", request).Error("Error while searching jobs")
			panic(err2)
		}
	}

	return &scrapedResults, nil
}

func (m *MonsterClient) searchJobs(request *monsterApiSearchJobsRequest) (*monsterApiSearchJobsResponse, error) {
	apiUrl := m.apiConfig.searchJobsUrl
	method := http.MethodPost

	jsonPayload, marshalErr := json.Marshal(request)
	if marshalErr != nil {
		panic(marshalErr)
	}
	payload := bytes.NewReader(jsonPayload)

	client := &http.Client{}
	req, err := http.NewRequest(method, *apiUrl, payload)

	if err != nil {
		log.WithFields(log.Fields{
			"payload": *payload,
			"method":  method,
			"url":     *apiUrl,
		}).Error(m.logWithName("Error while creating request"))
		panic(err)
	}
	req.Header.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0")
	req.Header.Add("Accept", "application/json, text/plain, */*")
	req.Header.Add("Accept-Language", "en-US,fr;q=0.8,fr-FR;q=0.5,en;q=0.3")
	req.Header.Add("Referer", "https://www.monster.fr/emploi/recherche/?q=d%C3%A9veloppeur&where=bordeaux&cy=fr&stpage=10&page=10")
	req.Header.Add("Content-Type", "application/json;charset=utf-8")
	req.Header.Add("Origin", "https://www.monster.fr")
	req.Header.Add("Connection", "keep-alive")
	req.Header.Add("TE", "Trailers")

	res, err := client.Do(req)
	if err != nil {
		log.WithFields(log.Fields{
			"request": req,
		}).Error(m.logWithName("Error while requesting api"))
		panic(err)
	}
	defer func() {
		err = res.Body.Close()
		if err != nil {
			log.WithField("body", *res).Error(m.logWithName("Error while closing the request body"))
			panic(err)
		}
	}()

	body, err := ioutil.ReadAll(res.Body)
	if err != nil {
		log.WithField("body", res.Body).Error(m.logWithName("Error while reading body"))
		panic(err)
	}

	var result monsterApiSearchJobsResponse
	unmarshalErr := json.Unmarshal(body, &result)
	if unmarshalErr != nil {
		log.WithField("body", string(body)).Error(m.logWithName("Cannot unmarshal body"))
		panic(unmarshalErr)
	}

	if err := recover(); err != nil {
		return nil, err.(error)
	}

	return &result, nil

}

func (m *MonsterClient) logWithName(msg string) string {
	return fmt.Sprintf("%s: %s", m.config.SiteName, msg)
}
