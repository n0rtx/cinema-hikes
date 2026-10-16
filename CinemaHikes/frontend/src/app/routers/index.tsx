import { createBrowserRouter } from "react-router-dom";

import { CatalogPage } from "../../pages/public/CatalogPage";
import { HomePage } from "../../pages/public/HomePage";
import { SupportPage } from "../../pages/public/SupportPage";
import { MainLayout } from "../../layouts/MainLayout";
import { MoviePage } from "../../pages/public/MoviePage";
import { AboutPage } from "../../pages/public/AboutPage";
import { DeveloperPage } from "../../pages/public/DeveloperPage";
import { NotFoundPage } from "../../pages/public/NotFoundPage";
import { LoginPage } from "../../pages/auth/LoginPage";
import { RegisterPage } from "../../pages/auth/RegisterPage";
import { ProfilePage } from "../../pages/private/ProfilePage";
import { UnderConstructionPage } from "../../pages/public/UnderConstructionPage";
import { GitHubCallbackPage } from "../../pages/auth/GithubCallbackPage";
import { AdminProtectedRoute } from "./ProtectedRoute";
import { AdminLayout } from "../../layouts/AdminLayout";
import { MoviesMgmtPage } from "../../pages/admin/MoviesMgmtPage";
import { AnalyticsMgmtPage } from "../../pages/admin/AnalyticsMgmtPage";
import { ReviewMgmtPage } from "../../pages/admin/UsersMgmtPage";
export const router = createBrowserRouter([
  { path: "/register", element: <RegisterPage /> },
  { path: "/login", element: <LoginPage /> },
  {
    path: "/admin",
    element: (
      <AdminProtectedRoute requireAdmin>
        <AdminLayout />
      </AdminProtectedRoute>
    ),
    children: [
      { path: "movies", element: <MoviesMgmtPage /> },
      { path: "analytic", element: <AnalyticsMgmtPage /> },
      {path:"reviews",element:<ReviewMgmtPage />}
    ],
  },
  { path: "/auth/github/callback", element: <GitHubCallbackPage /> },
  {
    path: "/",
    element: <MainLayout />,
    children: [
      { index: true, element: <HomePage /> },
      { path: "catalog", element: <CatalogPage /> },
      { path: "profile", element: <ProfilePage /> },
      { path: "movies/:id", element: <MoviePage /> },
      { path: "about", element: <AboutPage /> },
      { path: "developer/:username", element: <DeveloperPage /> },
      { path: "admin/test", element: <MoviesMgmtPage /> },
      { path: "support", element: <SupportPage /> },
      { path: "*", element: <NotFoundPage /> },
      { path: "developing", element: <UnderConstructionPage /> },
    ],
  },
]);
