package env

import (
	"os"
	"strconv"
)

func IsRunningOnLambda() *bool {
	onLambda := false
	onLambdaEnvVar := os.Getenv("ON_LAMBDA")
	if onLambdaEnvVar != "" {
		lb, pErr := strconv.ParseBool(onLambdaEnvVar)
		onLambda = lb
		if pErr != nil {
			onLambda = false
		}
	}
	return &onLambda
}
