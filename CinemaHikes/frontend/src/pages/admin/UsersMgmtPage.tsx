import { fetchUsers } from "../../features/admin/api/Users/fetchUsers";
import { AssignRole } from "../../features/admin/api/Users/AssignRole";
import { RemoveRole } from "../../features/admin/api/Users/RemoveRole";
import { useMutation, useQuery } from "@tanstack/react-query";
import type { AssignRoleDto } from "../../shared/types/DTO/AdminDtos/AssignRoleDto";
import { message } from "antd";

export const UsersMgmtPage = () => {
  const { data: users = [], isLoading } = useQuery({
    queryKey: ["admin-users"],
    queryFn: fetchUsers,
  });
  const removeRole = useMutation({
    mutationFn: (dto: AssignRoleDto) => RemoveRole(dto),
    onSuccess: () => {
      message.success("Роль успешно удаленна.");
    },
    onError: () => {
      message.error("Не удалось удалить роль.");
    },
  });
  const assignRole = useMutation({
    mutationFn: (dto: AssignRoleDto) => AssignRole(dto),
    onSuccess: () => {
      message.success("Роль успешно добавленна.");
    },
    onError: () => {
      message.error("Не удалось добавить роль.");
    },
  });
};
