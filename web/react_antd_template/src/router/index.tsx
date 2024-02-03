import {createBrowserRouter} from "react-router-dom";
import React, {ReactNode} from "react";
import {RouteGuard} from "./RouteGuard.tsx";

const Home =  React.lazy(()=> import("../views/Home"));
const Login = React.lazy(()=>import("../views/Login"));
const Page1 = React.lazy(()=>import("../views/Page1"));
const Page2 =  React.lazy(()=>import("../views/Page2"));
const NotFoundPage =  React.lazy(()=>import("../components/NotFoundPage"));

export interface RouteInfo {
    path: string;
    name: string;
    element: ReactNode;
    meta?: Meta;
    children?: RouteInfo[];
}

export interface Meta {
    title?: string;
    isAuth?: boolean;
}

// export function generateRouter(routes: RouteInfo[]): import("react-router-dom").RouteObject[] {
//     return routes.map((route) => {
//       const obj: import("react-router-dom").RouteObject = {
//         path: route.path,
//         element: route.element,
//       };
//       if (route.children) {
//         obj.children = generateRouter(route.children);
//       }
//       return obj;
//     });
//   }


export const routes: RouteInfo[] = [
    {
        path: "/",
        name: "home",
        element: <RouteGuard><Home/></RouteGuard>,
        children: [
            {
                path: "page1",
                name: "Page1",
                element: <Page1/>,
                meta: {
                    title: "Page1",
                    isAuth: true,
                }
            },
            {
                path: "page2",
                name: "Page2",
                element: <Page2/>,
                meta: {
                    title: "Page2",
                    isAuth: true,
                }
            },
        ],
        meta: {
            isAuth: true,
            title: "Home",
        }
    },
    {
        path: "/login",
        name: "login",
        element: <Login/>,
        meta: {
            title: "Login",
        }
    },
    {
        path: "*",
        name: "NotFound",
        element: <NotFoundPage/>,
        meta: {
            title: "NotFound",
        }
    },
];


export const router=createBrowserRouter(routes);
