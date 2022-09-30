import {component$, useContext} from "@builder.io/qwik";
import type {DocumentHead} from "@builder.io/qwik-city";
import {Link} from "@builder.io/qwik-city";
import {CompaniesData, HelloResponse, MyContext} from "~/root";



export async function getCompanies(
	controller?: AbortController
): Promise<CompaniesData> {

	const resp = await fetch("https://jobs.samuelbagattin.com/api/companies", {
		signal: controller?.signal,
	});
	return (await resp.json());

}


export default component$(() => {

	const state = useContext<HelloResponse>(MyContext);


	return (
		<div>
			<h1>
				Welcome to Qwik <span class="lightning">⚡️</span>
			</h1>

			<ul>
				<ul>
					{state.data.map((company) => (
						<li>
							<Link href={`companies/${company.Id}`}>
								{company.CompanyName}
							</Link>
						</li>
					))}
				</ul>


			</ul>
			{/*<ul>*/}
			{/*	{state.data.Companies.Companies.map((company: any) => (*/}
			{/*		<li><Link href={`companies/${company.CompanyName}`}>{company.CompanyName}</Link></li>))}*/}
			{/*</ul>*/}

			<h2>Commands</h2>
			<Link class="mindblow" href="/flower">
				Blow my mind 🤯
			</Link>
		</div>
	);
});

export const head: DocumentHead = {
	title: "Welcome to Qwik",
};
