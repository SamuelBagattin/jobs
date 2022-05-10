ARG GO_VERSION=1.18
FROM golang:${GO_VERSION}-bullseye AS build

WORKDIR /go/src/app
ADD . .

RUN go get -d -v ./...

RUN CGO_ENABLED=0 go build -installsuffix 'static' -o /jobs_scraper ./entrypoints/sqs_worker

FROM gcr.io/distroless/static AS final

USER nonroot:nonroot

COPY --from=build --chown=nonroot:nonroot /jobs_scraper /jobs_scraper

ENTRYPOINT ["/jobs_scraper"]