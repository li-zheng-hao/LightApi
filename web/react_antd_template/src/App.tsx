import { observer } from "mobx-react";
import React from "react";
import { RouteGuard } from "./router/RouteGuard";
import { useRoutes } from "react-router-dom";
import { generateRouter, routes } from "./router";
import { ConfigProvider } from "antd";

const App = () => {
  const elements = useRoutes(generateRouter(routes));
  return (
    <>
    <ConfigProvider theme={{ cssVar: true }}>
      <React.Suspense fallback={""}>
        <RouteGuard>{elements}</RouteGuard>
      </React.Suspense>
    </ConfigProvider>

    </>
  );
};

export default observer(App);
