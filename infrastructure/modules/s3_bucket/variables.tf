variable "bucket_name" {
  description = "s3 Bucket name"
  type        = string
}

variable "bucket_policy" {
  description = "JSON bucket policy"
  type        = string
  default     = null
}

variable "set_bucket_policy" {
  description = "Set s3 bucket policy"
  type        = bool
  default     = false
}

variable "access_logs_bucket_name" {
  description = "Access logs bucket name"
  type        = string
  default     = null
}

variable "objects_expiration_days" {
  description = "Objects expiration days"
  type        = number
  default     = null
}