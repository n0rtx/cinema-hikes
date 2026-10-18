import { axiosClient } from "../../../../shared/api/axiosClient";
import type { AssignRoleDto } from "../../../../shared/types/DTO/AdminDtos/AssignRoleDto";

export const RemoveRole = async(dto:AssignRoleDto):Promise<void> => {
    await axiosClient.post("api/AdminUser/remove-role",dto);
}