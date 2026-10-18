import { axiosClient } from "../../../../shared/api/axiosClient";

export const AssignRole = async(userId:number,rolename:string):Promise<void> => {
    await axiosClient.post("")
}