import {jwtDecode} from "jwt-decode";
interface CustomJwt {
    nameid?:string;
    exp?: number;
    email?:string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string | string[];
    role?:string|string[];
}
export const getUserRoles = (token:string | null):string[] => {
    if(!token) return [];
    try {

        const decode = jwtDecode<CustomJwt>(token);
        if(decode.exp && decode.exp * 1000 < Date.now()) {
            localStorage.removeItem("token");
            return [];
        }
        const roles = decode["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || decode.role;
        if (!roles) return [];
        return Array.isArray(roles) ? roles : [roles];
    }
    catch {
        return[];
    }
}
export const hasRole = (token:string | null,roleName:string):boolean => {
    const roles = getUserRoles(token);
    return roles.includes(roleName);
}