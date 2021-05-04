resource "aws_ssm_parameter" "scraping_queries" {
  name = "jobs-scraping-queries"
  type = "String"
  value = jsonencode([
    "developpeur",
    ".netcore",
    "java",
    "php",
    "javascript",
    "devops",
    "aws",
    "google cloud",
    "azure",
    "python",
    "iot",
    "c#",
    "nodejs",
    "vuejs",
    "react",
    "angular",
    "developpeur go",
    "golang",
    "symfony",
    "laravel",
    "c++",
    "android",
    "ios",
    "swift",
    "arduino",
    "tensorflow",
    "sql",
    "jenkins",
    "chef de projet",
    "alternance",
    "projet informatique"
  ])
}
