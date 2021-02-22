export interface Site {
  SiteName: string;
  JobUrl: string;
}

export interface Job {
  Id: string;
  PrimaryTechnologies: string[];
  SecondaryTechnologies: string[];
  Site: Site[];
  Title: string;
}

export interface Datum {
  Id: string;
  PrimaryTechnologies: string[];
  SecondaryTechnologies: string[];
  Company: string;
  Jobs: Job[];
}

export interface Companies {
  Data: Datum[];
  Count: number;
}

export interface JobsWithMainTechnology {
  Count: number;
}

export interface JobsWithSecondaryTechnology {
  Count: number;
}

export interface CompaniesWithSecondaryTechnologies {
  Count: number;
}

export interface CompaniesWithPrimaryTechnologies {
  Count: number;
}

export interface Datum2 {
  TechnologyName: string;
  JobsWithMainTechnology: JobsWithMainTechnology;
  JobsWithSecondaryTechnology: JobsWithSecondaryTechnology;
  CompaniesWithSecondaryTechnologies: CompaniesWithSecondaryTechnologies;
  CompaniesWithPrimaryTechnologies: CompaniesWithPrimaryTechnologies;
}

export interface Technologies {
  Data: Datum2[];
  TechnologiesCount: number;
}

export interface RootObject {
  Companies: {
    Data: Datum[];
    Count: number;
  };
  Technologies: Technologies;
}
