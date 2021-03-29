package upload

import (
	"bytes"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/s3"
	"github.com/aws/aws-sdk-go/service/s3/s3manager"
	"jobs_scaper/pkg/env"
	"log"
)

func NewS3Client(sess *session.Session) *S3ScrapingClient {
	return &S3ScrapingClient{
		s3uploader: s3manager.NewUploader(sess),
		config: &S3ScrapingClientConfig{
			BucketName: env.GetUploadBucketName(),
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
	BucketName *string
}

func (s *S3ScrapingClient) UploadToFileString(fileName string, content string, contentType string) {
	var contentBytes = []byte(content)
	s.UploadToFileByte(fileName, contentBytes, contentType)
}

func (s *S3ScrapingClient) UploadToFileByte(fileName string, content []byte, contentType string) {
	if len(content) == 0 {
		return
	}

	response, errr := s.s3.ListObjects(&s3.ListObjectsInput{
		Bucket: s.config.BucketName,
	})
	if errr != nil {
		panic(errr)
	}

	objectExists := false

	for _, object := range response.Contents {
		if *object.Key == fileName {
			objectExists = true
		}
	}

	if objectExists {
		log.Printf("%s already exists in bucket %s, deleting", fileName, *s.config.BucketName)
		_, err := s.s3.DeleteObject(&s3.DeleteObjectInput{
			Bucket: s.config.BucketName,
			Key:    &fileName,
		})
		if err != nil {
			panic(err)
		}
	}

	_, err := s.s3uploader.Upload(&s3manager.UploadInput{
		Bucket:      s.config.BucketName,
		Key:         aws.String(fileName),
		Body:        bytes.NewReader(content),
		ContentType: &contentType,
	})
	if err != nil {
		panic(err)
	}
}
