import { axiosClient } from "../../../../shared/api/axiosClient";
import type { UserDto } from "../../../../shared/types/DTO/UserDtos/UserDto";

export const fetchUsers = async():Promise<UserDto[]> => {
    const response  = await axiosClient.get<UserDto[]>("/api/AdminUser");
    return response.data;
}
