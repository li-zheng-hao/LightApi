import React, { Suspense } from 'react';
import ReactDOM from 'react-dom/client';
import { App, ConfigProvider } from 'antd';
import { RouterProvider } from 'react-router-dom';
import { router } from './router';
import './index.css';
import GlobalMessage from '@/utils/message';
ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <ConfigProvider theme={{ cssVar: true }}>
      <App>
        <GlobalMessage />
        <Suspense>
          <RouterProvider router={router}></RouterProvider>
        </Suspense>
      </App>
    </ConfigProvider>
  </React.StrictMode>
);
