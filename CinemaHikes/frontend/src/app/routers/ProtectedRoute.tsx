import { Navigate, Outlet } from "react-router-dom";
import { hasRole } from "../../shared/utils/hasRole";

export const AdminProtectedRoute = () => {
  const token = localStorage.getItem("token");

  
  const isAdmin = hasRole(token, "Admin");

  if (!isAdmin) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
};