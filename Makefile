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

# dirs
INFRA_DIR ?= $(abspath infrastructure)
FRONTEND_DIR ?= $(abspath frontend)
AGGREGATOR_DIR ?= $(abspath Jobs.Aggregator)
SCRAPER_DIR ?= $(abspath jobs_scraper)
BOT_DIR ?= $(abspath bot)
OUT_DIR ?= $(abspath out)

# terraform
TFVARS ?= $(INFRA_DIR)/prod.tfvars
AWS_PROFILE ?= "samuel"

.PHONY: install-tools
install-tools:
	$(BREW) bundle

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
apply: lint build-frontend build-bot build-aggregator
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(TERRAGRUNT) apply

# lint
.PHONY: lint
lint:
	cd $(INFRA_DIR) && $(TERRAGRUNT) fmt -recursive
	cd $(INFRA_DIR) && $(TERRAGRUNT) hclfmt
	cd $(INFRA_DIR) && AWS_PROFILE=$(AWS_PROFILE) $(TERRAGRUNT) validate
	cd $(INFRA_DIR) && $(TFLINT) --init -c ./tflint.hcl
	cd $(INFRA_DIR) && $(TFLINT) -c ./tflint.hcl



.PHONY: build
build: build-aggregator build-bot build-frontend build-scraper

.PHONY: build-aggregator
build-aggregator:
	cd $(AGGREGATOR_DIR) && $(DOTNET) build -c Release -o $(OUT_DIR)/aggregator

.PHONY: build-scraper
build-scraper:
	cd $(SCRAPER_DIR) && $(GO) build -o $(OUT_DIR)/scraper/scraper  ./entrypoints/sqs_worker

.PHONY: build-bot
build-bot:
	cd $(BOT_DIR) && $(YARN) install --frozen-lockfile

.PHONY: build-frontend
build-frontend:
	cd $(FRONTEND_DIR) && $(YARN) install --frozen-lockfile --silent
	cd $(FRONTEND_DIR) && $(YARN) --silent run build


.PHONY: run-bot-local
run-bot-local:
	cd $(BOT_DIR) && \
	AWS_PROFILE=$(AWS_PROFILE) \
	AWS_REGION=eu-west-3 \
	TOKEN_SSM_PARAM_NAME=/jobs/discordbot/token \
	PASTEEE_TOKEN_SSM_PARAM_NAME=/jobs/discordbot/pastee_token \
	$(YARN) run local



