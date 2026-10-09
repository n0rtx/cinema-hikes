import { useState } from "react";
import { Layout, Menu, Button, Popover, Grid, Drawer } from "antd";
import { Outlet, useLocation, useNavigate } from "react-router-dom";
import {
  UserOutlined,
  SearchOutlined,
  ShareAltOutlined,
  SendOutlined,
  MenuOutlined,
  GlobalOutlined,
} from "@ant-design/icons";
import { useTranslation } from "react-i18next";

const { Header, Content, Footer } = Layout;
const { useBreakpoint } = Grid;

export const MainLayout = () => {
  const { t, i18n } = useTranslation();

  const navigate = useNavigate();
  const location = useLocation();
  const screens = useBreakpoint();
  const [drawerOpen, setDrawerOpen] = useState(false);

  const currentLang = i18n.language.startsWith("uk") ? "UA" : "RU";
  const changeLanguage = (lng: "ru" | "uk") => {
    i18n.changeLanguage(lng);
  };

  const menuItems = [
    { key: "/", label: t("nav.home") },
    { key: "/catalog", label: t("nav.catalog") },
    { key: "/about", label: t("nav.about") },
    { key: "/support", label: t("nav.support") },
  ];

  const languageContent = (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        gap: "4px",
        minWidth: "100px",
      }}
    >
      <Button
        type="text"
        onClick={() => changeLanguage("ru")}
        style={{
          color: currentLang === "RU" ? "#E50914" : "#fff",
          textAlign: "left",
        }}
      >
        Русский
      </Button>
      <Button
        type="text"
        onClick={() => changeLanguage("uk")}
        style={{
          color: currentLang === "UA" ? "#E50914" : "#fff",
          textAlign: "left",
        }}
      >
        Українська
      </Button>
    </div>
  );

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
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: screens.lg ? "flex-start" : "space-between",
          padding: screens.md ? "0 40px" : "0 16px",
          position: "sticky",
          top: 0,
          zIndex: 1000,
          width: "100%",
        }}
      >
        <div
          onClick={() => navigate("/")}
          style={{
            color: "#E50914",
            fontSize: screens.md ? "24px" : "20px",
            fontWeight: 900,
            letterSpacing: "1px",
            marginRight: screens.md ? "40px" : "auto",
            cursor: "pointer",
          }}
        >
          PIRAT.tv
        </div>

        {screens.lg && (
          <Menu
            theme="dark"
            mode="horizontal"
            selectedKeys={[location.pathname]}
            defaultSelectedKeys={["home"]}
            items={menuItems}
            onClick={({ key }) => navigate(key)}
            style={{ flex: 1, borderBottom: "none" }}
          />
        )}

        <div
          style={{
            display: "flex",
            gap: screens.md ? "12px" : "4px",
            alignItems: "center",
          }}
        >
          <Popover
            content={languageContent}
            title={<span style={{ color: "#fff" }}>{t("nav.chooseLanguage")}</span>}
            trigger="click"
            placement="bottomRight"
            overlayInnerStyle={{
              backgroundColor: "#1f1f1f",
              border: "1px solid #333",
            }}
          >
            <Button
              type="text"
              style={{
                color: "#fff",
                display: "flex",
                alignItems: "center",
                gap: "6px",
                padding: screens.md ? "4px 12px" : "4px 6px",
              }}
            >
              <GlobalOutlined style={{ fontSize: "16px", color: "#E50914" }} />
              <span style={{ fontSize: "13px", fontWeight: 600 }}>
                {currentLang}
              </span>
            </Button>
          </Popover>

          <Popover
            content={socialContent}
            title={<span style={{ color: "#fff" }}>{t("nav.subscribe")}</span>}
            trigger="hover"
            placement="bottomRight"
            overlayInnerStyle={{
              backgroundColor: "#1f1f1f",
              border: "1px solid #333",
            }}
          >
            <Button
              type="text"
              style={{
                color: "#fff",
                display: "flex",
                alignItems: "center",
                gap: "6px",
                padding: screens.md ? "4px 12px" : "4px 6px",
              }}
            >
              <ShareAltOutlined
                style={{ fontSize: "18px", color: "#E50914" }}
              />
              {screens.md && <span>{t("nav.subscribe")}</span>}
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
            onClick={() => navigate("/profile")}
          />

          {!screens.lg && (
            <Button
              type="text"
              icon={
                <MenuOutlined style={{ fontSize: "20px", color: "#fff" }} />
              }
              onClick={() => setDrawerOpen(true)}
            />
          )}
        </div>
      </Header>

      <Drawer
        title={
          <span
            style={{ color: "#E50914", fontWeight: 900, letterSpacing: "1px" }}
          >
            PIRAT.tv
          </span>
        }
        placement="right"
        onClose={() => setDrawerOpen(false)}
        open={drawerOpen}
        styles={{
          body: { padding: 0, backgroundColor: "#141414" },
          header: {
            backgroundColor: "#1f1f1f",
            borderBottom: "1px solid #333",
          },
        }}
      >
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[location.pathname]}
          items={menuItems}
          onClick={({ key }) => {
            navigate(key);
            setDrawerOpen(false);
          }}
          style={{ borderRight: "none", backgroundColor: "#141414" }}
        />
      </Drawer>

      <Content
        style={{
          padding: screens.md ? "40px" : "24px 16px",
          maxWidth: "1440px",
          margin: "0 auto",
          width: "100%",
        }}
      >
        <Outlet />
      </Content>

      <Footer
        style={{
          textAlign: "center",
          color: "#666",
          padding: screens.md ? "24px 50px" : "24px 16px",
        }}
      >
        CinemaHikes ©{new Date().getFullYear()}
      </Footer>
    </Layout>
  );
};