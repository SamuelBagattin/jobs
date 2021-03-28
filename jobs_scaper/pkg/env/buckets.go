package env

import (
	"errors"
	"fmt"
	"os"
)

const (
	ResultsBucketEnvVarName = "RESULTS_BUCKET_NAME"
)

func GetUploadBucketName() *string {
	bucketNameEnvVar := os.Getenv(ResultsBucketEnvVarName)
	if bucketNameEnvVar != "" {
		return &bucketNameEnvVar
	}
	panic(errors.New(fmt.Sprintf("Missing Env Var: %s", ResultsBucketEnvVarName)))
}
