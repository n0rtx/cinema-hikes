import { useState, useEffect } from "react";
import {
  Form,
  Input,
  Button,
  Typography,
  message,
  Checkbox,
  Spin,
  Alert,
} from "antd";
import { MailOutlined, LockOutlined, PlayCircleOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import axios from "axios";

const { Title, Text } = Typography;

interface LoginFormValues {
  email: string;
  password: string;
  remember: boolean;
}

const useMediaQuery = (query: string) => {
  const [matches, setMatches] = useState(
    () => typeof window !== "undefined" && window.matchMedia(query).matches,
  );

  useEffect(() => {
    const mql = window.matchMedia(query);
    const handler = (e: MediaQueryListEvent) => setMatches(e.matches);
    setMatches(mql.matches);
    mql.addEventListener("change", handler);
    return () => mql.removeEventListener("change", handler);
  }, [query]);

  return matches;
};

export const LoginPage = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const { t } = useTranslation();
  const isWide = useMediaQuery("(min-width: 1200px)");

  const onFinish = async (values: LoginFormValues) => {
    setLoading(true);
    setError(null);

    try {
      const response = await axios.post(
        "http://localhost:5000/api/auth/login",
        {
          email: values.email,
          password: values.password,
        },
        {
          headers: {
            "Content-Type": "application/json",
          },
        },
      );

      localStorage.setItem("authToken", response.data.token);
      localStorage.setItem("username", response.data.username);
      localStorage.setItem("email", response.data.email);

      if (values.remember) {
        localStorage.setItem("rememberMe", "true");
      }

      message.success(t("auth.welcomeBack"));

      setTimeout(() => {
        navigate("/");
      }, 1500);
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message || t("auth.loginFailed");
      setError(errorMessage);
      message.error(errorMessage);
      console.error("Login error:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      style={{
        display: "flex",
        minHeight: "100vh",
        backgroundColor: "#141414",
      }}
    >
      {isWide && (
        <div
          style={{
            flex: "0 0 58.333%",
            backgroundColor: "#1f1f1f",
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            padding: "40px",
            borderRight: "1px solid #333",
            boxSizing: "border-box",
          }}
        >
          <div style={{ textAlign: "center", maxWidth: "500px" }}>
            <div style={{ fontSize: "64px", marginBottom: "32px" }}>
              <PlayCircleOutlined style={{ color: "#E50914" }} />
            </div>

            <Title level={2} style={{ color: "#fff", marginBottom: "16px" }}>
              {t("auth.loginHeroTitle")}
            </Title>

            <Text style={{ color: "#aaa", fontSize: "16px", lineHeight: "1.6" }}>
              {t("auth.loginHeroText")}
            </Text>

            <div style={{ marginTop: "40px", display: "flex", gap: "16px" }}>
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: "28px", color: "#E50914", fontWeight: "bold" }}>
                  10K+
                </div>
                <Text style={{ color: "#999", fontSize: "14px" }}>
                  {t("auth.statTitles")}
                </Text>
              </div>
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: "28px", color: "#E50914", fontWeight: "bold" }}>
                  50K+
                </div>
                <Text style={{ color: "#999", fontSize: "14px" }}>
                  {t("auth.statCaptains")}
                </Text>
              </div>
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: "28px", color: "#E50914", fontWeight: "bold" }}>
                  24/7
                </div>
                <Text style={{ color: "#999", fontSize: "14px" }}>
                  {t("auth.statSupport")}
                </Text>
              </div>
            </div>
          </div>
        </div>
      )}

      <div
        style={{
          flex: 1,
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          padding: "32px 16px",
          boxSizing: "border-box",
          minWidth: 0,
        }}
      >
        <div
          style={{
            maxWidth: "380px",
            width: "100%",
            margin: "0 auto",
            boxSizing: "border-box",
          }}
        >
          <div
            onClick={() => navigate("/")}
            style={{
              color: "#E50914",
              fontSize: "24px",
              fontWeight: 900,
              letterSpacing: "1px",
              fontFamily:
                "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', sans-serif, 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol', 'Noto Color Emoji'",
              cursor: "pointer",
              marginBottom: "24px",
              lineHeight: 1.2,
            }}
          >
            PIRAT.tv
          </div>

          <div style={{ marginBottom: "32px" }}>
            <Title level={2} style={{ color: "#fff", marginBottom: "8px" }}>
              {t("auth.loginTitle")}
            </Title>
            <Text style={{ color: "#999", fontSize: "14px" }}>
              {t("auth.loginSubtitle")}
            </Text>
          </div>

          {error && (
            <Alert
              message={t("auth.loginError")}
              description={error}
              type="error"
              showIcon
              closable
              onClose={() => setError(null)}
              style={{ marginBottom: "20px" }}
            />
          )}

          <Spin spinning={loading}>
            <Form
              form={form}
              onFinish={onFinish}
              autoComplete="off"
              layout="vertical"
            >
              <Form.Item
                name="email"
                label={<span style={{ color: "#fff" }}>{t("auth.email")}</span>}
                rules={[
                  {
                    required: true,
                    message: t("auth.emailRequired"),
                  },
                ]}
              >
                <Input
                  prefix={<MailOutlined />}
                  placeholder={t("auth.emailPlaceholder")}
                  size="large"
                  autoComplete="email"
                  style={{
                    backgroundColor: "#333",
                    border: "1px solid #555",
                    color: "#fff",
                  }}
                />
              </Form.Item>

              <Form.Item
                name="password"
                label={<span style={{ color: "#fff" }}>{t("auth.password")}</span>}
                rules={[
                  {
                    required: true,
                    message: t("auth.passwordRequired"),
                  },
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder={t("auth.passwordPlaceholder")}
                  size="large"
                  autoComplete="current-password"
                  style={{
                    backgroundColor: "#333",
                    border: "1px solid #555",
                  }}
                />
              </Form.Item>

              <Form.Item
                name="remember"
                valuePropName="checked"
                style={{ marginBottom: "16px" }}
              >
                <Checkbox style={{ color: "#999" }}>{t("auth.rememberMe")}</Checkbox>
              </Form.Item>

              <div style={{ marginBottom: "20px", textAlign: "right" }}>
                <span
                  onClick={() => navigate("/forgot-password")}
                  style={{
                    color: "#E50914",
                    textDecoration: "none",
                    fontSize: "14px",
                    cursor: "pointer",
                    transition: "color 0.3s",
                  }}
                  onMouseOver={(e) => {
                    (e.currentTarget as HTMLElement).style.color = "#ff4d4f";
                  }}
                  onMouseOut={(e) => {
                    (e.currentTarget as HTMLElement).style.color = "#E50914";
                  }}
                >
                  {t("auth.forgotPassword")}
                </span>
              </div>

              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  size="large"
                  block
                  loading={loading}
                  style={{
                    backgroundColor: "#E50914",
                    borderColor: "#E50914",
                    height: "44px",
                    fontSize: "16px",
                    fontWeight: "600",
                  }}
                >
                  {loading ? t("auth.signingIn") : t("auth.loginButton")}
                </Button>
              </Form.Item>
            </Form>
          </Spin>

          <div style={{ textAlign: "center", marginTop: "20px" }}>
            <Text style={{ color: "#999" }}>
              {t("auth.noAccount")}{" "}
              <span
                onClick={() => navigate("/register")}
                style={{
                  color: "#E50914",
                  textDecoration: "none",
                  fontWeight: "600",
                  cursor: "pointer",
                  transition: "color 0.3s",
                }}
                onMouseOver={(e) => {
                  (e.currentTarget as HTMLElement).style.color = "#ff4d4f";
                }}
                onMouseOut={(e) => {
                  (e.currentTarget as HTMLElement).style.color = "#E50914";
                }}
              >
                {t("auth.joinUs")}
              </span>
            </Text>
          </div>
        </div>
      </div>
    </div>
  );
};