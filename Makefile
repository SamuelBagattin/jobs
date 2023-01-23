# tools
TERRAFORM ?= $(shell which terraform)
TERRAGRUNT ?= $(shell which terragrunt)
MAKE ?= $(shell which make)
AWS ?= $(shell which aws)
TFLINT ?= $(shell which tflint)
TFSEC ?= $(shell which tfsec)
BREW ?= $(shell which brew)
PRE_COMMIT_HOOK ?= $(shell which pre-commit)
DOTNET ?= $(shell which dotnet)
GO ?= $(shell which go)
YARN ?= $(shell which yarn)
NPM ?= $(shell which npm)
POETRY ?= $(shell which poetry)
DOCKER ?= $(shell which docker)

# dirs
INFRA_DIR ?= $(abspath infrastructure)
FRONTEND_DIR ?= $(abspath frontend)
AGGREGATOR_DIR ?= $(abspath Jobs.Aggregator/Jobs.Aggregator.Local)
SCRAPER_DIR ?= $(abspath jobs_scraper)
BOT_DIR ?= $(abspath bot)
QUERIER_DIR ?= $(abspath querier)
OUT_DIR ?= $(abspath out)

# terraform
TFVARS ?= $(INFRA_DIR)/prod.tfvars
AWS_PROFILE ?= "samuel"

.PHONY: install-tools
install-tools:
	$(BREW) bundle
	$(DOTNET) tool update --global Amazon.Lambda.Tools

.PHONY: login
login:
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(AWS) sso login

.PHONY: init
init: install-tools
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(TERRAGRUNT) init

.PHONY: init-upgrade
init-upgrade: install-tools
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(TERRAGRUNT) init -upgrade

.PHONY: plan
plan:
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(TERRAGRUNT) plan

.PHONY: apply
apply: init lint build-bot build-aggregator
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(TERRAGRUNT) apply

# lint
.PHONY: lint
lint:
	cd $(INFRA_DIR) && $(TERRAGRUNT) fmt -recursive
	cd $(INFRA_DIR) && $(TERRAGRUNT) hclfmt
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(TERRAGRUNT) validate
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(TFLINT) --init -c ./tflint.hcl
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(TFLINT) -c ./tflint.hcl



.PHONY: build
build: build-aggregator build-bot build-frontend build-scraper build-querier

.PHONY: build-aggregator
build-aggregator:
	# Native AOT build does not work on Lambda "/var/task/bootstrap: /lib64/libm.so.6: version `GLIBC_2.29' not found (required by /var/task/bootstrap)"
    #$(DOCKER) run --platform linux/amd64 --entrypoint sh -v $$(pwd):/application mcr.microsoft.com/dotnet/sdk:7.0 -c "apt update && apt install -y clang zip llvm zlib1g-dev && dotnet publish -r linux-x64 -c Release -o /application/out/aggregator /application/Jobs.Aggregator/Jobs.Aggregator.Local/Jobs.Aggregator.Local.csproj"
	cd $(AGGREGATOR_DIR) && $(DOTNET) publish -c Release -r linux-arm64 --self-contained -o $(OUT_DIR)/aggregator/

.PHONY: build-scraper
build-scraper:
	cd $(SCRAPER_DIR) && $(GO) build -o $(OUT_DIR)/scraper/scraper  ./entrypoints/sqs_worker

.PHONY: build-bot
build-bot:
	cd $(BOT_DIR) && $(YARN) install --frozen-lockfile

.PHONY: build-frontend
build-frontend:
	cd $(FRONTEND_DIR) && $(NPM) install && $(NPM) run build && $(NPM) run export
	cd $(FRONTEND_DIR)/infrastructure && $(NPM) install && $(NPM) run build


.PHONY: run-bot-local
run-bot-local:
	cd $(BOT_DIR) && \
	AWS_PROFILE=$(AWS_PROFILE) \
	AWS_REGION=eu-west-3 \
	TOKEN_SSM_PARAM_NAME=/jobs/discordbot/token \
	PASTEEE_TOKEN_SSM_PARAM_NAME=/jobs/discordbot/pastee_token \
	$(YARN) run local

.PHONY: build-querier
build-querier:
	cd $(QUERIER_DIR) && rm -rf package dist
	cd $(QUERIER_DIR) && $(POETRY) install && $(POETRY) build && $(POETRY) run pip install -t package dist/*.whl



