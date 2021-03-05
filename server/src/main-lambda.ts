import {ServerConfig} from "./graphql";

const {ApolloServer} = require('apollo-server-lambda');
const server = new ApolloServer({...ServerConfig, ...{playground: {endpoint: "/prod"}}});

exports.graphqlHandler = server.createHandler();
