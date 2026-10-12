
import { Navigate } from "react-router-dom";

interface Props{
    children:React.ReactNode,
    requireAdmin?:boolean
}

export const ProtectedRoute = ({children,requireAdmin}:Props) => {
    const token = localStorage.getItem("authToken");
    const role = localStorage.getItem("role")
    if(!token) {
        return <Navigate to="/login" replace/>
    }
    if(requireAdmin && role !== "Admin") {
         return <Navigate to="/" replace/> 
    }
    return <>{children}</>;
}