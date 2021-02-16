package upload

import (
	"bytes"
	"fmt"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/s3"
	"github.com/aws/aws-sdk-go/service/s3/s3manager"
	"log"
	"time"
)

func NewS3Client(sess *session.Session) *S3ScrapingClient {
	return &S3ScrapingClient{
		s3uploader: s3manager.NewUploader(sess),
		config: &S3ScrapingClientConfig{
			BucketName: "jobs-scraper-results",
		},
		s3: s3.New(sess),
	}
}

type S3ScrapingClient struct {
	s3uploader *s3manager.Uploader
	config     *S3ScrapingClientConfig
	s3         *s3.S3
}

type S3ScrapingClientConfig struct {
	BucketName string
}

func (s *S3ScrapingClient) UploadToFileString(fileName string, content string) {
	var contentBytes = []byte(content)
	s.UploadToFileByte(fileName, contentBytes)
}

func (s *S3ScrapingClient) UploadToFileByte(fileName string, content []byte) {
	if len(content) == 0 {
		return
	}

	var date = time.Now()
	var prefix = fmt.Sprintf("%d-%d-%d", date.Year(), date.Month(), date.Day())
	var objectKey = prefix + "/" + fileName

	response, errr := s.s3.ListObjects(&s3.ListObjectsInput{
		Bucket: aws.String(s.config.BucketName),
		Prefix: aws.String(prefix),
	})
	if errr != nil {
		panic(errr)
	}

	objectExists := false

	for _, object := range response.Contents {
		if *object.Key == objectKey {
			objectExists = true
		}
	}

	if objectExists {
		log.Printf("%s already exists in bucket %s, deleting", objectKey, s.config.BucketName)
		_, err := s.s3.DeleteObject(&s3.DeleteObjectInput{
			Bucket: aws.String(s.config.BucketName),
			Key:    &objectKey,
		})
		if err != nil {
			panic(err)
		}
	}

	_, err := s.s3uploader.Upload(&s3manager.UploadInput{
		Bucket: aws.String(s.config.BucketName),
		Key:    aws.String(objectKey),
		Body:   bytes.NewReader(content),
	})
	if err != nil {
		panic(err)
	}
}
