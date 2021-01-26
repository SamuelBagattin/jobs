CREATE TABLE Jobs
(
    Id      SERIAL PRIMARY KEY,
    Title   VARCHAR(500),
    Company VARCHAR(50),
    Site VARCHAR(50),
    Url VARCHAR(5000),
    ScrapeDate Date
);
