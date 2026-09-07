import { Layout, Menu, Button,Popover } from "antd";

import { Outlet, useLocation, useNavigate } from "react-router-dom";

import {
  UserOutlined,
  SearchOutlined,
  ShareAltOutlined,
  SendOutlined,
} from "@ant-design/icons";
const { Header, Content, Footer } = Layout;
export const MainLayout = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const menuItems = [
    { key: "/", label: "Home" },
    { key: "/catalog", label: "Catalog" },
    { key: "/about", label: "About us" },
  ];
  const socialContent = (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        gap: "8px",
        minWidth: "140px",
      }}
    >
      <a
        href="https://t.me/cinema_hikes_downloader_bot"
        target="_blank"
        rel="noopener noreferrer"
        style={{
          color: "#fff",
          display: "flex",
          alignItems: "center",
          gap: "8px",
          textDecoration: "none",
        }}
      >
        <SendOutlined style={{ color: "#0088cc" }} /> Telegram
      </a>
    </div>
  );
  return (
    <Layout style={{ minHeight: "100vh" }}>
      <Header
        style={{ display: "flex", alignItems: "center", padding: "0 40px" }}
      >
        <div
          style={{
            color: "#E50914",
            fontSize: "24px",
            fontWeight: 900,
            letterSpacing: "1px",
            marginRight: "40px",
            cursor: "pointer",
          }}
        >
          PIRAT.tv
        </div>
        <Menu
          theme="dark"
          mode="horizontal"
          selectedKeys={[location.pathname]}
          defaultSelectedKeys={["home"]}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
          style={{ flex: 1, borderBottom: "none" }}
        />
        <div style={{ display: "flex", gap: "16px", alignItems: "center" }}>
          <Popover
            content={socialContent}
            title={<span style={{ color: "#fff" }}>Follow Us</span>}
            trigger="hover"
            placement="bottomRight"
            overlayInnerStyle={{
              backgroundColor: "#1f1f1f",
              border: "1px solid #333",
            }}
          >
            <Button
              type="text"
              icon={
                <ShareAltOutlined
                  style={{ fontSize: "18px", color: "#E50914" }}
                />
              }
              style={{
                color: "#fff",
                display: "flex",
                alignItems: "center",
                gap: "6px",
              }}
            >
              Follow
            </Button>
          </Popover>
          <Button
            type="text"
            icon={
              <SearchOutlined style={{ fontSize: "18px", color: "#fff" }} />
            }
          />
          <Button
            type="text"
            icon={<UserOutlined style={{ fontSize: "18px", color: "#fff" }} />}
          />
        </div>
      </Header>
      <Content
        style={{
          padding: "40px",
          maxWidth: "1440px",
          margin: "0 auto",
          width: "100%",
        }}
      >
        <Outlet /> {}
      </Content>
      <Footer style={{ textAlign: "center", color: "#666" }}>
        CinemaHikes ©{new Date().getFullYear()} — FindYourMovie
      </Footer>
    </Layout>
  );
};
