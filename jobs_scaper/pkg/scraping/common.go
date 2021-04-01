package scraping

import (
	"net/url"
	"time"
)

type JobInfo struct {
	Title       string
	Company     string
	Site        string
	Url         string
	Date        time.Time
	Id          string
	Description string
}

type ClientConfig struct {
	SiteName     string
	SiteOrigin   string
	SiteBasePath *url.URL
}

type Client interface {
	GetConfig() *ClientConfig
	Scrape() (*[]*JobInfo, error)
	logWithName(msg string) string
}
