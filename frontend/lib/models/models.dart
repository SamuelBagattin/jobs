class JobsRootObject {
  Companies companies;
  Technologies technologies;

  JobsRootObject({this.companies, this.technologies});

  JobsRootObject.fromJson(Map<String, dynamic> json) {
    companies = json['Companies'] != null
        ? new Companies.fromJson(json['Companies'])
        : null;
    technologies = json['Technologies'] != null
        ? new Technologies.fromJson(json['Technologies'])
        : null;
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    if (this.companies != null) {
      data['Companies'] = this.companies.toJson();
    }
    if (this.technologies != null) {
      data['Technologies'] = this.technologies.toJson();
    }
    return data;
  }
}

class Companies {
  List<Company> companies;
  int count;

  Companies({this.companies, this.count});

  Companies.fromJson(Map<String, dynamic> json) {
    if (json['Companies'] != null) {
      companies = <Company>[];
      json['Companies'].forEach((v) {
        companies.add(new Company.fromJson(v));
      });
    }
    count = json['Count'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    if (this.companies != null) {
      data['Companies'] = this.companies.map((v) => v.toJson()).toList();
    }
    data['Count'] = this.count;
    return data;
  }
}

class Company {
  String id;
  List<String> mainTechnologies;
  List<String> secondaryTechnologies;
  String companyName;
  List<Jobs> jobs;

  Company(
      {this.id,
      this.mainTechnologies,
      this.secondaryTechnologies,
      this.companyName,
      this.jobs});

  Company.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    mainTechnologies = json['MainTechnologies'].cast<String>();
    secondaryTechnologies = json['SecondaryTechnologies'].cast<String>();
    companyName = json['CompanyName'];
    if (json['Jobs'] != null) {
      jobs = <Jobs>[];
      json['Jobs'].forEach((v) {
        jobs.add(new Jobs.fromJson(v));
      });
    }
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['MainTechnologies'] = this.mainTechnologies;
    data['SecondaryTechnologies'] = this.secondaryTechnologies;
    data['CompanyName'] = this.companyName;
    if (this.jobs != null) {
      data['Jobs'] = this.jobs.map((v) => v.toJson()).toList();
    }
    return data;
  }
}

class Jobs {
  String id;
  List<String> mainTechnologies;
  List<String> secondaryTechnologies;
  List<Site> site;
  String jobTitle;

  Jobs(
      {this.id,
      this.mainTechnologies,
      this.secondaryTechnologies,
      this.site,
      this.jobTitle});

  Jobs.fromJson(Map<String, dynamic> json) {
    id = json['Id'];
    mainTechnologies = json['MainTechnologies'].cast<String>();
    secondaryTechnologies = json['SecondaryTechnologies'].cast<String>();
    if (json['Site'] != null) {
      site = <Site>[];
      json['Site'].forEach((v) {
        site.add(new Site.fromJson(v));
      });
    }
    jobTitle = json['JobTitle'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Id'] = this.id;
    data['MainTechnologies'] = this.mainTechnologies;
    data['SecondaryTechnologies'] = this.secondaryTechnologies;
    if (this.site != null) {
      data['Site'] = this.site.map((v) => v.toJson()).toList();
    }
    data['JobTitle'] = this.jobTitle;
    return data;
  }
}

class Site {
  String siteName;
  String jobUrl;

  Site({this.siteName, this.jobUrl});

  Site.fromJson(Map<String, dynamic> json) {
    siteName = json['SiteName'];
    jobUrl = json['JobUrl'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['SiteName'] = this.siteName;
    data['JobUrl'] = this.jobUrl;
    return data;
  }
}

class Technologies {
  List<Technologies> technologies;
  int technologiesCount;

  Technologies({this.technologies, this.technologiesCount});

  Technologies.fromJson(Map<String, dynamic> json) {
    if (json['Technologies'] != null) {
      technologies = <Technologies>[];
      json['Technologies'].forEach((v) {
        technologies.add(new Technologies.fromJson(v));
      });
    }
    technologiesCount = json['TechnologiesCount'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    if (this.technologies != null) {
      data['Technologies'] = this.technologies.map((v) => v.toJson()).toList();
    }
    data['TechnologiesCount'] = this.technologiesCount;
    return data;
  }
}

class TechnologyStatistics {
  String technologyName;
  JobsWithMainTechnology jobsWithMainTechnology;
  JobsWithMainTechnology jobsWithSecondaryTechnology;
  JobsWithMainTechnology companiesWithSecondaryTechnologies;
  JobsWithMainTechnology companiesWithMainTechnologies;

  TechnologyStatistics(
      {this.technologyName,
      this.jobsWithMainTechnology,
      this.jobsWithSecondaryTechnology,
      this.companiesWithSecondaryTechnologies,
      this.companiesWithMainTechnologies});

  TechnologyStatistics.fromJson(Map<String, dynamic> json) {
    technologyName = json['TechnologyName'];
    jobsWithMainTechnology = json['JobsWithMainTechnology'] != null
        ? new JobsWithMainTechnology.fromJson(json['JobsWithMainTechnology'])
        : null;
    jobsWithSecondaryTechnology = json['JobsWithSecondaryTechnology'] != null
        ? new JobsWithMainTechnology.fromJson(
            json['JobsWithSecondaryTechnology'])
        : null;
    companiesWithSecondaryTechnologies =
        json['CompaniesWithSecondaryTechnologies'] != null
            ? new JobsWithMainTechnology.fromJson(
                json['CompaniesWithSecondaryTechnologies'])
            : null;
    companiesWithMainTechnologies =
        json['CompaniesWithMainTechnologies'] != null
            ? new JobsWithMainTechnology.fromJson(
                json['CompaniesWithMainTechnologies'])
            : null;
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['TechnologyName'] = this.technologyName;
    if (this.jobsWithMainTechnology != null) {
      data['JobsWithMainTechnology'] = this.jobsWithMainTechnology.toJson();
    }
    if (this.jobsWithSecondaryTechnology != null) {
      data['JobsWithSecondaryTechnology'] =
          this.jobsWithSecondaryTechnology.toJson();
    }
    if (this.companiesWithSecondaryTechnologies != null) {
      data['CompaniesWithSecondaryTechnologies'] =
          this.companiesWithSecondaryTechnologies.toJson();
    }
    if (this.companiesWithMainTechnologies != null) {
      data['CompaniesWithMainTechnologies'] =
          this.companiesWithMainTechnologies.toJson();
    }
    return data;
  }
}

class JobsWithMainTechnology {
  List<String> ids;
  int count;

  JobsWithMainTechnology({this.ids, this.count});

  JobsWithMainTechnology.fromJson(Map<String, dynamic> json) {
    ids = json['Ids'].cast<String>();
    count = json['Count'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['Ids'] = this.ids;
    data['Count'] = this.count;
    return data;
  }
}
