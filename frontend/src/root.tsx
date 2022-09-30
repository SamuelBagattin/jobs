import {component$, createContext, useContextProvider, useMount$, useStore} from "@builder.io/qwik";
import {
  QwikCity,
  RouterOutlet,
  ServiceWorkerRegister,
} from "@builder.io/qwik-city";
import { RouterHead } from "./components/router-head/router-head";

import "./global.css";
import {getCompanies} from "~/routes";

export interface HelloResponse {
    data: CompaniesData;
}

export type CompaniesData = {
    Id: string,
    CompanyName: string,
}[]

export const MyContext = createContext<HelloResponse>('my-context');

export default component$(() => {

    const state = useStore<{ data: CompaniesData }>({
        data: [],
    });

    useMount$(async () => {
        state.data = await getCompanies()
    });

    useContextProvider<HelloResponse>(MyContext, state);
  /**
   * The root of a QwikCity site always start with the <QwikCity> component,
   * immediately followed by the document's <head> and <body>.
   *
   * Dont remove the `<head>` and `<body>` elements.
   */
  return (
    <QwikCity>
      <head>
        <meta charSet="utf-8" />
        <RouterHead />
      </head>
      <body lang="en">
        <RouterOutlet />
        <ServiceWorkerRegister />
      </body>
    </QwikCity>
  );
});
