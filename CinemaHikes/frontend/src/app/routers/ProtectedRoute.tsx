import type { ReactNode } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { hasRole } from "../../shared/utils/hasRole";

interface ProtectedRouteProps {
  requireAdmin?: boolean;
  children?: ReactNode;
}

export const AdminProtectedRoute = ({
  requireAdmin = false,
  children,
}: ProtectedRouteProps) => {
  const token = localStorage.getItem("authToken");


  if (!token) {
    return <Navigate to="/login" replace />;
  }


  if (requireAdmin && !hasRole(token, "Admin")) {
    return <Navigate to="/" replace />;
  }


  return children ? <>{children}</> : <Outlet />;
};