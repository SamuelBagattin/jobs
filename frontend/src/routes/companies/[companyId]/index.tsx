import {component$, useResource$, Resource} from "@builder.io/qwik";
import {useLocation} from "@builder.io/qwik-city";
import {getJobs} from "~/services/jobsService";
import {JobData} from "~/models/JobData";

export default component$(() => {
	const location = useLocation();
	const reposResource = useResource$<JobData>(({cleanup}) => {
		const controller = new AbortController();
		cleanup(() => controller.abort());
		return getJobs(location.params.companyId, controller)
	});
	return (
		<>
			<div>
				<h1>{location.params.companyId}</h1>

					<Resource
						value={reposResource}
						onPending={() => <div>Loading...</div>}
						onRejected={(error) => <div>Error: {error.message}</div>}
						onResolved={(data) => <pre>{JSON.stringify(data, null, 2)}</pre>}
						/>

			</div>
		</>
	);
});
