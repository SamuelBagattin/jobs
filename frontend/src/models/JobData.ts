export interface JobData {
	Id: string,
	MainTechnologies: string[],
	SecondaryTechnologies: string[],
	Site: {
		SiteName: string,
		JobUrl: string,
	}[],
	JobTitle: string,
	Statistics: {
		Occurences: number,
	}
}