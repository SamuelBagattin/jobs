package main

import (
	"os"
	"strconv"
)

func isRunningOnLambda() *bool {
	onLambda := false
	onLambdaEnvVar := os.Getenv("ON_LAMBDA")
	if onLambdaEnvVar != "" {
		lb, pErr := strconv.ParseBool(onLambdaEnvVar)
		onLambda = lb
		if pErr != nil {
			panic(pErr)
		}
	}
	return &onLambda
}
