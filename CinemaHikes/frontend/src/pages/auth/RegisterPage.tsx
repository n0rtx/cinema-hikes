import { useState, useEffect } from "react";
import {
  Form,
  Input,
  Button,
  Typography,
  message,
  Divider,
  Spin,
  Alert,
} from "antd";
import {
  UserOutlined,
  LockOutlined,
  MailOutlined,
  GoogleOutlined,
  GithubOutlined,
  FacebookFilled,
  PlayCircleOutlined,
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import axios from "axios";

const { Title, Text } = Typography;

interface RegisterFormValues {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
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

export const RegisterPage = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const { t } = useTranslation();
  const isWide = useMediaQuery("(min-width: 1200px)");

  const onFinish = async (values: RegisterFormValues) => {
    setLoading(true);
    setError(null);

    try {
      const response = await axios.post(
        "http://localhost:5000/api/auth/register",
        {
          username: values.username,
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

      message.success(t("auth.welcomeAboard"));

      setTimeout(() => {
        navigate("/");
      }, 1500);
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message || t("auth.registerFailed");
      setError(errorMessage);
      message.error(errorMessage);
      console.error("Registration error:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleSocialLogin = (provider: string) => {
    message.info(t("auth.socialDev", { provider }));
  };

  const socialBtnStyle = {
    backgroundColor: "transparent",
    borderColor: "#E50914",
    color: "#E50914",
    height: "40px",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    borderRadius: "4px",
    cursor: "pointer",
    transition: "all 0.3s",
    border: "1px solid #E50914",
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
              {t("auth.registerHeroTitle")}
            </Title>

            <Text style={{ color: "#aaa", fontSize: "16px", lineHeight: "1.6" }}>
              {t("auth.registerHeroText")}
            </Text>

            <div style={{ marginTop: "40px", display: "flex", gap: "16px" }}>
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: "28px", color: "#E50914", fontWeight: "bold" }}>
                  10K+
                </div>
                <Text style={{ color: "#999", fontSize: "14px" }}>
                  {t("auth.statMovies")}
                </Text>
              </div>
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: "28px", color: "#E50914", fontWeight: "bold" }}>
                  100%
                </div>
                <Text style={{ color: "#999", fontSize: "14px" }}>
                  {t("auth.statFree")}
                </Text>
              </div>
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: "28px", color: "#E50914", fontWeight: "bold" }}>
                  HD
                </div>
                <Text style={{ color: "#999", fontSize: "14px" }}>
                  {t("auth.statHd")}
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
              {t("auth.registerHeroTitle")}
            </Title>
            <Text style={{ color: "#999", fontSize: "14px" }}>
              {t("auth.registerSubtitle")}
            </Text>
          </div>

          {error && (
            <Alert
              message={t("auth.registerError")}
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
                name="username"
                label={<span style={{ color: "#fff" }}>{t("auth.username")}</span>}
                rules={[
                  { required: true, message: t("auth.usernameRequired") },
                  { min: 3, message: t("auth.usernameMin") },
                  { max: 50, message: t("auth.usernameMax") },
                  {
                    pattern: /^[a-zA-Z0-9_-]+$/,
                    message: t("auth.usernamePattern"),
                  },
                ]}
              >
                <Input
                  prefix={<UserOutlined />}
                  placeholder={t("auth.usernamePlaceholder")}
                  size="large"
                  style={{
                    backgroundColor: "#333",
                    border: "1px solid #555",
                    color: "#fff",
                  }}
                />
              </Form.Item>

              <Form.Item
                name="email"
                label={<span style={{ color: "#fff" }}>{t("auth.email")}</span>}
                rules={[
                  { required: true, message: t("auth.emailRequired") },
                  { type: "email", message: t("auth.emailInvalid") },
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
                  { required: true, message: t("auth.passwordRequired") },
                  { min: 6, message: t("auth.passwordMin") },
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder={t("auth.passwordPlaceholder")}
                  size="large"
                  style={{
                    backgroundColor: "#333",
                    border: "1px solid #555",
                  }}
                />
              </Form.Item>

              <Form.Item
                name="confirmPassword"
                label={
                  <span style={{ color: "#fff" }}>{t("auth.confirmPassword")}</span>
                }
                dependencies={["password"]}
                rules={[
                  { required: true, message: t("auth.confirmRequired") },
                  ({ getFieldValue }) => ({
                    validator(_, value) {
                      if (!value || getFieldValue("password") === value) {
                        return Promise.resolve();
                      }
                      return Promise.reject(new Error(t("auth.passwordsMismatch")));
                    },
                  }),
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder={t("auth.confirmPlaceholder")}
                  size="large"
                  style={{
                    backgroundColor: "#333",
                    border: "1px solid #555",
                  }}
                />
              </Form.Item>

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
                  {loading ? t("auth.creatingAccount") : t("auth.registerButton")}
                </Button>
              </Form.Item>
            </Form>
          </Spin>

          <Divider style={{ backgroundColor: "#141414", margin: "24px 0" }}>
            <span style={{ color: "#999" }}>{t("auth.orContinueWith")}</span>
          </Divider>

          <div
            style={{
              display: "grid",
              gridTemplateColumns: "1fr 1fr 1fr",
              gap: "12px",
              marginBottom: "24px",
            }}
          >
            <button
              style={socialBtnStyle as any}
              onClick={() => handleSocialLogin("Google")}
              onMouseOver={(e) => {
                const target = e.currentTarget as HTMLElement;
                target.style.backgroundColor = "rgba(229, 9, 20, 0.1)";
              }}
              onMouseOut={(e) => {
                const target = e.currentTarget as HTMLElement;
                target.style.backgroundColor = "transparent";
              }}
            >
              <GoogleOutlined style={{ fontSize: "18px" }} />
            </button>
            <button
              style={socialBtnStyle as any}
              onClick={() => handleSocialLogin("GitHub")}
              onMouseOver={(e) => {
                const target = e.currentTarget as HTMLElement;
                target.style.backgroundColor = "rgba(229, 9, 20, 0.1)";
              }}
              onMouseOut={(e) => {
                const target = e.currentTarget as HTMLElement;
                target.style.backgroundColor = "transparent";
              }}
            >
              <GithubOutlined style={{ fontSize: "18px" }} />
            </button>
            <button
              style={socialBtnStyle as any}
              onClick={() => handleSocialLogin("Facebook")}
              onMouseOver={(e) => {
                const target = e.currentTarget as HTMLElement;
                target.style.backgroundColor = "rgba(229, 9, 20, 0.1)";
              }}
              onMouseOut={(e) => {
                const target = e.currentTarget as HTMLElement;
                target.style.backgroundColor = "transparent";
              }}
            >
              <FacebookFilled style={{ fontSize: "18px" }} />
            </button>
          </div>

          <div style={{ textAlign: "center" }}>
            <Text style={{ color: "#999" }}>
              {t("auth.hasAccount")}{" "}
              <span
                onClick={() => navigate("/login")}
                style={{
                  color: "#E50914",
                  textDecoration: "none",
                  fontWeight: "600",
                  cursor: "pointer",
                }}
              >
                {t("auth.signIn")}
              </span>
            </Text>
          </div>
        </div>
      </div>
    </div>
  );
};