import {jwtDecode} from "jwt-decode";
interface CustomJwt {
    nameid?:string;
    exp?: number;
    email?:string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string | string[];
    role?:string|string[];
}
export const getUserRoles = (authToken:string | null):string[] => {
    if(!authToken) return [];
    try {

        const decode = jwtDecode<CustomJwt>(authToken);
        if(decode.exp && decode.exp * 1000 < Date.now()) {
            localStorage.removeItem("authToken");
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
export const hasRole = (authToken:string | null,roleName:string):boolean => {
    const roles = getUserRoles(authToken);
    return roles.includes(roleName);
}