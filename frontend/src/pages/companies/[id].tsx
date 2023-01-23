import { GetStaticPaths, GetStaticProps } from 'next';
import { getCompanies } from '@/lib/companies';
import { getJobs, JobData } from '@/lib/jobs';

export default function Id({ jobs }: { jobs: JobData[] }) {
  return (
    <>
      <ul>
        {jobs.map((job) => (
          <>
            <li key={job.Id}>
              <div>
                <p>{job.JobTitle}</p>
                <div>
                  <ul>
                    {job.Site.map((site) => (
                      <a key={site.SiteName} href={site.JobUrl}>
                        {site.SiteName}
                      </a>
                    ))}
                  </ul>
                </div>
              </div>
            </li>
          </>
        ))}
      </ul>
    </>
  );
}

export const getStaticProps: GetStaticProps = async ({ params }) => {
  if (params && params[`id`]) {
    const companyId = params[`id`] as string;
    const jobs = await getJobs(companyId);
    return {
      props: {
        jobs: jobs,
      },
    };
  }
  return {
    props: {},
  };
};

export const getStaticPaths: GetStaticPaths = async () => {
  const companies = await getCompanies();
  const paths = companies.map((company) => ({
    params: { id: company.Id },
  }));
  return {
    paths,
    fallback: false,
  };
};
