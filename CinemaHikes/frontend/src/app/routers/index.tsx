import { createBrowserRouter } from "react-router-dom";

import { CatalogPage } from "../../pages/public/CatalogPage";
import { HomePage } from "../../pages/public/HomePage";
import { MainLayout } from "../../layouts/MainLayout";
import { AboutPage } from "../../pages/public/AboutPage";
import { DeveloperPage } from '../../pages/public/DeveloperPage';
import { NotFoundPage } from "../../pages/public/NotFoundPage";
import { LoginPage } from "../../pages/auth/LoginPage";
import { RegisterPage } from "../../pages/auth/RegisterPage";
export const router = createBrowserRouter([
  {path:"/register",element:<RegisterPage/>},
  { 
    path: "/login", 
    element: <LoginPage /> 
  },
  {
    path: "/",
    element: <MainLayout />,
    children: [
      { index: true, element: <HomePage /> },
      { path: "catalog", element: <CatalogPage /> },
      { path: "about", element: <AboutPage /> },
      { path: 'developer/:username', element: <DeveloperPage /> },
      {path:"*",element:<NotFoundPage/>},
    ],
  },
]);
