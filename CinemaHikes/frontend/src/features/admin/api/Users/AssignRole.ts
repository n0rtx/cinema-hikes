import { axiosClient } from "../../../../shared/api/axiosClient";
import type { AssignRoleDto } from "../../../../shared/types/DTO/AdminDtos/AssignRoleDto";

export const AssignRole = async (dto: AssignRoleDto): Promise<void> => {
  await axiosClient.post("/api/AdminUser/add-role", dto);
};
