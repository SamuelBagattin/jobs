export async function getJobs(
  companyId: string,
  controller?: AbortController,
): Promise<JobData> {
  console.log(`https://jobs.samuelbagattin.com/api/companies/${companyId}`);
  const resp = await fetch(
    `https://jobs.samuelbagattin.com/api/companies/${companyId}`,
    {
      signal: controller?.signal,
    },
  );
  return (await resp.json()) as JobData;
}

export interface JobData {
  Id: string;
  MainTechnologies: string[];
  SecondaryTechnologies: string[];
  Site: {
    SiteName: string;
    JobUrl: string;
  }[];
  JobTitle: string;
  Statistics: {
    Occurences: number;
  };
}
