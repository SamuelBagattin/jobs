import {JobData} from "~/models/JobData";

export async function getJobs(
	companyId: string,
	controller?: AbortController
): Promise<JobData> {
console.log(`https://jobs.samuelbagattin.com/api/companies/${companyId}`)
	const resp = await fetch(`https://jobs.samuelbagattin.com/api/companies/${companyId}`, {
		signal: controller?.signal,
	});
	return (await resp.json()) as JobData;

}