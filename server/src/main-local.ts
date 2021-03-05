// The ApolloServer constructor requires two parameters: your schema
// definition and your set of resolvers.
import {ApolloServer} from "apollo-server";
import {ServerConfig} from "./graphql";

const server = new ApolloServer(ServerConfig);

// The `listen` method launches a web server.
server.listen().then(({url}) => {
    console.log(`🚀  Server ready at ${url}`);
});
