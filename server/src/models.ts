export interface Site {
    SiteName: string;
    JobUrl: string;
}

export interface Job {
    Id: string;
    MainTechnologies: string[];
    SecondaryTechnologies: string[];
    Site: Site[];
    JobTitle: string;
}

export interface Company {
    Id: string;
    MainTechnologies: string[];
    SecondaryTechnologies: string[];
    CompanyName: string;
    Jobs: Job[];
}

export interface Companies {
    Companies: Company[];
    Count: number;
}

export interface JobsWithMainTechnology {
    Ids: string[];
    Count: number;
}

export interface JobsWithSecondaryTechnology {
    Ids: string[];
    Count: number;
}

export interface CompaniesWithSecondaryTechnologies {
    Ids: string[];
    Count: number;
}

export interface CompaniesWithMainTechnologies {
    Ids: string[];
    Count: number;
}

export interface Technology {
    TechnologyName: string;
    JobsWithMainTechnology: JobsWithMainTechnology;
    JobsWithSecondaryTechnology: JobsWithSecondaryTechnology;
    CompaniesWithSecondaryTechnologies: CompaniesWithSecondaryTechnologies;
    CompaniesWithMainTechnologies: CompaniesWithMainTechnologies;
}

export interface Technologies {
    Technologies: Technology[];
    TechnologiesCount: number;
}

export interface RootObject {
    Companies: Companies;
    Technologies: Technologies;
}


