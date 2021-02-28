import {ApolloServer, gql} from "apollo-server"
import axios from "axios";
import {data, setup} from "./data";
// A schema is a collection of type definitions (hence "typeDefs")
// that together define the "shape" of queries that are executed against
// your data.
setup().then()

const typeDefs = gql`
    # Comments in GraphQL strings (such as this one) start with the hash (#) symbol.

    # This "Book" type defines the queryable fields for every book in our data source.
    type Companies {
        Count: Int
        Data: [Company]
    }
    
    type Company {
        Id: String
        PrimaryTechnologies: [String]
        SecondaryTechnologies: [String]
        Company: String
        Jobs: [Job]
    }
    type Job{
        Id: String
        PrimaryTechnologies: [String]
        SecondaryTechnologies: [String]
        Site: [Site]
    }
    type Site{
        SiteName: String
        JobUrl: String
    }

    # The "Query" type is special: it lists all of the available queries that
    # clients can execute, a
    type Query {
        companies: Companies
        company(Id: String!): Company
    }
`;

// Resolvers define the technique for fetching the types defined in the
// schema. This resolver retrieves books from the "books" array above.
const resolvers = {
    Query: {
        companies: () => data().Companies,
        company: (parent, args) => data().Companies.Data.find(e => e.Id === args.Id)
    },
    Companies: {
        Count: (parent) => parent.Count,
        Data: (parent) => parent.Data
    },
    Company: {
        Id: (parent) => parent.Id,
        Company: (parent) => parent.Company
    }

};

// The ApolloServer constructor requires two parameters: your schema
// definition and your set of resolvers.
const server = new ApolloServer({
    typeDefs,
    resolvers,
});

// The `listen` method launches a web server.
server.listen().then(({url}) => {
    console.log(`🚀  Server ready at ${url}`);
});
