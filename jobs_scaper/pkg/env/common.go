package env

import (
	"errors"
	"fmt"
	"os"
)

func getEnvVar(name *string) *string {
	envVar := os.Getenv(*name)
	if envVar != "" {
		return &envVar
	}
	fmt.Println(errors.New(fmt.Sprintf("Missing Env Var: %s", *name)))
	os.Exit(1)
	return nil
}
