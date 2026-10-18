import { Layout, Menu } from "antd";
import { Outlet, useNavigate, useLocation } from "react-router-dom";
import {
  DashboardOutlined,
  VideoCameraOutlined,
  TeamOutlined,
  CommentOutlined,
} from "@ant-design/icons";
const { Sider, Content } = Layout;

export const AdminLayout = () => {
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const items = [
    { key: "/admin/analytic", icon: <DashboardOutlined />, label: "Дашборд" },
    { key: "/admin/movies", icon: <VideoCameraOutlined />, label: "Фильмы" },
    { key: "/admin/users", icon: <TeamOutlined />, label: "Пользователи" },
    { key: "/admin/reviews", icon: <CommentOutlined  />, label: "Отзывы" },
  ];
  return (
    <Layout style={{ minHeight: "100vh", background: "#141414" }}>
      <Sider theme="dark" style={{ background: "#1c1c1c" }}>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[pathname]}
          items={items}
          onClick={({ key }) => navigate(key)}
          style={{ background: "#1c1c1c" }}
        />
      </Sider>
      <Content style={{ padding: 24 }}>
        <Outlet />
      </Content>
    </Layout>
  );
};
