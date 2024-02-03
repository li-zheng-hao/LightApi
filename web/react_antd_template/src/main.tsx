import React from "react";
import ReactDOM from "react-dom/client";
import {App, ConfigProvider} from "antd";
import {RouterProvider} from "react-router-dom";
import {router} from "./router";
import './index.css'
import 'uno.css'
import GlobalMessage from "@/utils/message"
ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
      <ConfigProvider theme={{cssVar: true}}>
          <App>
              <GlobalMessage/>
              <RouterProvider router={router}></RouterProvider>
          </App>
      </ConfigProvider>
  </React.StrictMode>
);
