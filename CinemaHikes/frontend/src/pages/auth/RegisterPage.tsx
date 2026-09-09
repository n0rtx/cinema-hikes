import { useState } from "react";
import {
  Form,
  Input,
  Button,
  Typography,
  message,
  Row,
  Col,
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
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import axios from "axios";

const { Title, Text } = Typography;

interface RegisterFormValues {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export const RegisterPage = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

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
        }
      );

      localStorage.setItem("authToken", response.data.token);
      localStorage.setItem("username", response.data.username);
      localStorage.setItem("email", response.data.email);

      message.success("Welcome aboard, Captain! ");

      setTimeout(() => {
        navigate("/");
      }, 1500);
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message || "Registration failed. Please try again later.";
      setError(errorMessage);
      message.error(errorMessage);
      console.error("Registration error:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleSocialLogin = (provider: string) => {
    message.info(`Sign in with ${provider} is under development...`);
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
  };

  return (
    <Row style={{ minHeight: "100vh", backgroundColor: "#141414" }}>
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
        {/* <div style={{ textAlign: "center", maxWidth: "80%" }}>
          <img
            src="https://via.placeholder.com/600x600/141414/E50914?text=CinemaHikes+Register"
            alt="CinemaHikes Register"
            style={{
              maxWidth: "100%",
              borderRadius: "24px",
              boxShadow: "0 20px 40px rgba(229, 9, 20, 0.3)",
            }}
          />
        </div> */}
      </Col>

      <Col
        xs={24}
        md={12}
        lg={10}
        style={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          padding: "40px 8%",
        }}
      >
        <div style={{ maxWidth: "380px", width: "100%", margin: "0 auto" }}>
          <div 
            onClick={() => navigate('/')} 
            style={{ color: '#E50914', fontSize: '28px', fontWeight: 900, cursor: 'pointer', marginBottom: '24px' }}
          >
            PIRAT.tv
          </div>

          <div style={{ marginBottom: "32px" }}>
            <Title level={2} style={{ color: "#fff", marginBottom: "8px" }}>
              Join the Crew
            </Title>
            <Text style={{ color: "#999", fontSize: "14px" }}>
              Create an account and start your journey
            </Text>
          </div>

          {error && (
            <Alert
              message="Registration Error"
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
                label={<span style={{ color: "#fff" }}>Username</span>}
                rules={[
                  {
                    required: true,
                    message: "Please enter your username",
                  },
                  {
                    min: 3,
                    message: "Username must be at least 3 characters",
                  },
                  {
                    max: 50,
                    message: "Username cannot exceed 50 characters",
                  },
                  {
                    pattern: /^[a-zA-Z0-9_-]+$/,
                    message:
                      "Username can only contain letters, numbers, hyphens, and underscores",
                  },
                ]}
              >
                <Input
                  prefix={<UserOutlined />}
                  placeholder="pirat_123"
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
                label={<span style={{ color: "#fff" }}>Email</span>}
                rules={[
                  {
                    required: true,
                    message: "Please enter your email",
                  },
                  {
                    type: "email",
                    message: "Please enter a valid email address",
                  },
                ]}
              >
                <Input
                  prefix={<MailOutlined />}
                  placeholder="captain@example.com"
                  size="large"
                  type="email"
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
                  {
                    min: 6,
                    message: "Password must be at least 6 characters",
                  },
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="••••••••"
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
                  <span style={{ color: "#fff" }}>Confirm Password</span>
                }
                dependencies={["password"]}
                rules={[
                  {
                    required: true,
                    message: "Please confirm your password",
                  },
                  ({ getFieldValue }) => ({
                    validator(_, value) {
                      if (!value || getFieldValue("password") === value) {
                        return Promise.resolve();
                      }
                      return Promise.reject(
                        new Error("The two passwords that you entered do not match")
                      );
                    },
                  }),
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="••••••••"
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
                  {loading ? "Signing up..." : "Sign Up"}
                </Button>
              </Form.Item>
            </Form>
          </Spin>

          <Divider style={{ backgroundColor: "#333", margin: "24px 0" }}>
            <span style={{ color: "#999" }}>or</span>
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
              Already have an account?{" "}
              <span
                onClick={() => navigate("/login")}
                style={{
                  color: "#E50914",
                  textDecoration: "none",
                  fontWeight: "600",
                  cursor: "pointer",
                }}
              >
                Log In
              </span>
            </Text>
          </div>
        </div>
      </Col>
    </Row>
  );
};