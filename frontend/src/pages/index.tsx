import Head from 'next/head';
import Image from 'next/image';

import { GetStaticProps } from 'next';
import { CompaniesData, getCompanies } from '@/lib/companies';
import Link from 'next/link';

export default function Home({ companies }: { companies: CompaniesData }) {
  return (
    <div>
      <Head>
        <title>TypeScript starter for Next.js</title>
        <meta
          name="description"
          content="TypeScript starter for Next.js that includes all you need to build amazing apps"
        />
        <link rel="icon" href="/favicon.ico" />
      </Head>
      <header style={{ backgroundColor: `bisque` }}>Jobs at Bordeaux</header>

      <main style={{ backgroundColor: `aqua` }}>
        <ul>
          {companies.map((company) => (
            <li key={company.Id}>
              <Link href={`/companies/${company.Id}`} prefetch={false}>
                <a>{company.CompanyName}</a>
              </Link>
            </li>
          ))}
        </ul>
      </main>

      <footer style={{ backgroundColor: `navajowhite` }}>
        <a
          href="https://vercel.com?utm_source=typescript-nextjs-starter"
          target="_blank"
          rel="noopener noreferrer"
        >
          Powered by{` `}
          <span>
            <Image src="/vercel.svg" alt="Vercel Logo" width={72} height={16} />
          </span>
        </a>
      </footer>
    </div>
  );
}

export const getStaticProps: GetStaticProps = async (context) => {
  const companies = await getCompanies();
  return {
    props: {
      companies,
    },
  };
};
