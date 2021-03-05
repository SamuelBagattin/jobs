import {gql} from "apollo-server"
import {data} from "./data";
import {
    Companies,
    CompaniesWithMainTechnologies,
    CompaniesWithSecondaryTechnologies,
    Company, JobsWithMainTechnology, JobsWithSecondaryTechnology,
    Technologies
} from "./models";
// A schema is a collection of type definitions (hence "typeDefs")
// that together define the "shape" of queries that are executed against
// your data.
// setup().then()

const typeDefs = gql`
    type CompaniesWithMainTechnologies { Count: Int Companies: [Company ] }

    type CompaniesWithSecondaryTechnologies { Count: Int Companies: [Company ] }

    type JobsWithSecondaryTechnology { Count: Int Companies: [Company ] }

    type JobsWithMainTechnology { Count: Int Companies: [Company ] }

    type Technology {
        TechnologyName: String
        CompaniesWithMainTechnologies: CompaniesWithMainTechnologies
        CompaniesWithSecondaryTechnologies: CompaniesWithSecondaryTechnologies
        JobsWithSecondaryTechnology: JobsWithSecondaryTechnology
        JobsWithMainTechnology: JobsWithMainTechnology
    }

    type Site {
        SiteName: String
        JobUrl: String
    }

    type Job {
        Id: String
        JobTitle: String
        Site: [Site ]
        SecondaryTechnologies: [String ]
        MainTechnologies: [String ]
    }

    type Company {
        Id: String
        CompanyName: String
        Jobs: [Job]
        SecondaryTechnologies: [String]
        MainTechnologies: [String]
    }

    type Companies{
        Companies: [Company]
        Count: Int
    }
    type Technologies{
        Technologies: [Technology]
        TechnologiesCount: Int
    }

    type Root {
        Technologies: Technologies
        Companies: Companies
    }

    # Types with identical fields:
    # CompaniesWithMainTechnologies CompaniesWithSecondaryTechnologies JobsWithSecondaryTechnology JobsWithMainTechnology

    # The "Query" type is special: it lists all of the available queries that
    # clients can execute, a
    type Query {
        companies: Companies
        company(Id: String!): Company
        technologies: Technologies
    }
`;

// Resolvers define the technique for fetching the types defined in the
// schema. This resolver retrieves books from the "books" array above.
const resolvers = {
    Query: {
        companies: (): Companies => data().Companies,
        company: (parent, args: { Id: String }): Company | undefined => data().Companies.Companies.find(e => e.Id === args.Id),
        technologies: (): Technologies => data().Technologies
    },
    CompaniesWithMainTechnologies: {
        Companies: (parent: CompaniesWithMainTechnologies): Company[] => data().Companies.Companies.filter(e => parent.Ids.indexOf(e.Id) != -1)
    },
    CompaniesWithSecondaryTechnologies: {
        Companies: (parent: CompaniesWithSecondaryTechnologies): Company[] => data().Companies.Companies.filter(e => parent.Ids.some(d => d === e.Id))
    },
    JobsWithSecondaryTechnology: {
        Companies: (parent: JobsWithSecondaryTechnology): Company[] => data().Companies.Companies.filter(e => parent.Ids.some(d => d === e.Id))
    },
    JobsWithMainTechnology: {
        Companies: (parent: JobsWithMainTechnology): Company[] => data().Companies.Companies.filter(e => parent.Ids.some(d => d === e.Id))
    }

};
export const ServerConfig =  {
    typeDefs,
    resolvers,
}

