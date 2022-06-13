package utils

import (
	"fmt"
	log "github.com/sirupsen/logrus"
	"math/rand"
	"os"
	"time"
	"unsafe"
)

const letterBytes = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
const (
	letterIdxBits = 6                    // 6 bits to represent a letter index
	letterIdxMask = 1<<letterIdxBits - 1 // All 1-bits, as many as letterIdxBits
	letterIdxMax  = 63 / letterIdxBits   // # of letter indices fitting in 63 bits
)

var src = rand.NewSource(time.Now().UnixNano())

// RandStringBytesMaskImprSrcUnsafe https://stackoverflow.com/questions/22892120/how-to-generate-a-random-string-of-a-fixed-length-in-go
func RandStringBytesMaskImprSrcUnsafe(n int) string {
	b := make([]byte, n)
	// A src.Int63() generates 63 random bits, enough for letterIdxMax characters!
	for i, cache, remain := n-1, src.Int63(), letterIdxMax; i >= 0; {
		if remain == 0 {
			cache, remain = src.Int63(), letterIdxMax
		}
		if idx := int(cache & letterIdxMask); idx < len(letterBytes) {
			b[i] = letterBytes[idx]
			i--
		}
		cache >>= letterIdxBits
		remain--
	}

	return *(*string)(unsafe.Pointer(&b))
}

// RandScrapingInterval Generates duration between 0.5 and 1.5 seconds
func RandScrapingInterval() time.Duration {
	i := int(rand.Float32()*1000 + 2000)
	return time.Millisecond * time.Duration(i)
}

func ExecuteWithRetries(executor func() error, retries int) error {
	actualRetries := 0
	for err := executor(); err != nil; {
		if actualRetries == retries {
			log.Error("Retries count exceeded")
			return err
		}
		actualRetries++
		time.Sleep(RandScrapingInterval())
		log.Debug("Retrying...")
	}
	return nil
}

func WriteToFile(filename string, content string) error {
	f, err := os.Create(filename)
	if err != nil {
		return err
	}
	_, err = f.WriteString(content)
	if err != nil {
		fmt.Println(err)
		closeErr := f.Close()
		if closeErr != nil {
			return closeErr
		}
		return err
	}
	err = f.Close()
	if err != nil {
		return err
	}
	return nil
}
