export async function getCompanies(
  controller?: AbortController,
): Promise<CompaniesData> {
  const resp = await fetch(`https://jobs.samuelbagattin.com/api/companies`, {
    signal: controller?.signal,
  });
  return await resp.json();
}

export type CompaniesData = {
  Id: string;
  CompanyName: string;
}[];
