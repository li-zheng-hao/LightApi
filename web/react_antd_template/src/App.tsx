import { observer } from "mobx-react";
import React from "react";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import router from "./router";
import { ConfigProvider } from "antd";



const App = () => {
  return (
    <>
    <ConfigProvider theme={{ cssVar: true }}>
      <React.Suspense fallback={""}>
          <RouterProvider router={router} />
      </React.Suspense>
    </ConfigProvider>

    </>
  );
};

export default observer(App);
