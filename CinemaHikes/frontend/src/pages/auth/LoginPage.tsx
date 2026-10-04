import { useState } from "react";
import {
  Form,
  Input,
  Button,
  Typography,
  message,
  Row,
  Col,
  Checkbox,
  Spin,
  Alert,
} from "antd";
import { MailOutlined, LockOutlined, PlayCircleOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import axios from "axios";

const { Title, Text } = Typography;

interface LoginFormValues {
  email: string;
  password: string;
  remember: boolean;
}

export const LoginPage = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

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

      message.success("Welcome back, Captain! 🏴‍☠️");

      setTimeout(() => {
        navigate("/");
      }, 1500);
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message ||
        "Login failed. Please check your email and password.";
      setError(errorMessage);
      message.error(errorMessage);
      console.error("Login error:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Row style={{ minHeight: "100vh", backgroundColor: "#141414" }}>
      {/* Left Side - Marketing */}
      <Col
        xs={0}
        md={12}
        lg={14}
        style={{
          backgroundColor: "#1f1f1f",
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          padding: "40px",
          borderRight: "1px solid #333",
        }}
      >
        <div
          style={{
            textAlign: "center",
            maxWidth: "500px",
          }}
        >
          <div
            style={{
              fontSize: "64px",
              marginBottom: "32px",
            }}
          >
            <PlayCircleOutlined style={{ color: "#E50914" }} />
          </div>

          <Title level={2} style={{ color: "#fff", marginBottom: "16px" }}>
            Your Cinema Awaits
          </Title>

          <Text style={{ color: "#aaa", fontSize: "16px", lineHeight: "1.6" }}>
            Continue your adventure through our vast collection of movies and shows. Access your watchlist, recommendations, and more.
          </Text>

          <div style={{ marginTop: "40px", display: "flex", gap: "16px" }}>
            <div style={{ flex: 1 }}>
              <div style={{ fontSize: "28px", color: "#E50914", fontWeight: "bold" }}>
                10K+
              </div>
              <Text style={{ color: "#999", fontSize: "14px" }}>
                Titles Available
              </Text>
            </div>
            <div style={{ flex: 1 }}>
              <div style={{ fontSize: "28px", color: "#E50914", fontWeight: "bold" }}>
                50K+
              </div>
              <Text style={{ color: "#999", fontSize: "14px" }}>
                Active Captains
              </Text>
            </div>
            <div style={{ flex: 1 }}>
              <div style={{ fontSize: "28px", color: "#E50914", fontWeight: "bold" }}>
                24/7
              </div>
              <Text style={{ color: "#999", fontSize: "14px" }}>
                Support
              </Text>
            </div>
          </div>
        </div>
      </Col>

      {/* Right Side - Form */}
      <Col
        xs={24}
        md={12}
        lg={10}
        style={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          padding: "32px 16px",
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
              fontSize: "28px",
              fontWeight: 900,
              cursor: "pointer",
              marginBottom: "24px",
            }}
          >
            PIRAT.tv
          </div>

          <div style={{ marginBottom: "32px" }}>
            <Title level={2} style={{ color: "#fff", marginBottom: "8px" }}>
              Welcome Back
            </Title>
            <Text style={{ color: "#999", fontSize: "14px" }}>
              Sign in to your account to continue
            </Text>
          </div>

          {error && (
            <Alert
              message="Login Error"
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
                label={<span style={{ color: "#fff" }}>Email or Username</span>}
                rules={[
                  {
                    required: true,
                    message: "Please enter your email or username",
                  },
                ]}
              >
                <Input
                  prefix={<MailOutlined />}
                  placeholder="captain@example.com"
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
                label={<span style={{ color: "#fff" }}>Password</span>}
                rules={[
                  {
                    required: true,
                    message: "Please enter your password",
                  },
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="••••••••"
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
                <Checkbox style={{ color: "#999" }}>Remember me</Checkbox>
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
                  Forgot password?
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
                  {loading ? "Signing in..." : "Log In"}
                </Button>
              </Form.Item>
            </Form>
          </Spin>

          <div style={{ textAlign: "center", marginTop: "20px" }}>
            <Text style={{ color: "#999" }}>
              Don't have an account?{" "}
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
                Sign Up
              </span>
            </Text>
          </div>
        </div>
      </Col>
    </Row>
  );
};