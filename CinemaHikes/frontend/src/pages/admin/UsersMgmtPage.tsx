import { fetchUsers } from "../../features/admin/api/Users/fetchUsers";
import { AssignRole } from "../../features/admin/api/Users/AssignRole";
import type { ColumnsType } from "antd/es/table";
import { RemoveRole } from "../../features/admin/api/Users/RemoveRole";
import { useMutation, useQuery } from "@tanstack/react-query";
import type { AssignRoleDto } from "../../shared/types/DTO/AdminDtos/AssignRoleDto";
import { message, Table, Tag, Typography } from "antd";

import type { UserDto } from "../../shared/types/DTO/UserDtos/UserDto";
const { Title } = Typography;
const statusColors: Record<string, string> = {
  "Admin":"gold",
  "User":"green",
  "Moderator":"blue",
};
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
  const colums: ColumnsType<UserDto> = [
    {
      title: "Id",
      dataIndex: "id",
      key: "id",
      render: (text: number) => <strong>{text}</strong>,
    },
    {
      title: "Username",
      dataIndex: "username",
      key: "username",
      render: (text: string) => <strong>{text}</strong>,
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
      render: (text: string) => <strong>{text}</strong>,
    },
    {
      title: "Role",
      dataIndex: "roles",
      key: "roles",
      render: (roles: string[]) =>
        roles?.length
          ? roles.map((r) => (
              <Tag key={r} color={statusColors[r]}>
                {r}
              </Tag>
            ))
          : "Нет ролей",
  ];
  return (
    <div>
      <div>
        <Title level={3} style={{ color: "#fff", margin: 0 }}>
          {"Наши пираты"}
        </Title>
      </div>
      <Table
        columns={colums}
        dataSource={users}
        rowKey="id"
        loading={isLoading}
      />
    </div>
  );
};
