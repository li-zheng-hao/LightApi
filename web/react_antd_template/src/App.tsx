import { observer } from "mobx-react";
import React from "react";
import {  RouteGuard } from "./router/RouteGuard";
import { useRoutes } from "react-router-dom";
import { generateRouter, routes } from "./router";

const App = () => {
  const elements = useRoutes(generateRouter(routes))
  return (
    <>
    <React.Suspense fallback={""}>
    <RouteGuard>
      {elements}
    </RouteGuard>
  </React.Suspense>
        
    </>
  );
};

export default observer(App);


