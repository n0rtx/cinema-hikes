import { useState, useEffect } from "react";
import { Typography, Row, Col, Card, Avatar, Button, Statistic, Spin, Divider } from "antd";
import { UserOutlined, MailOutlined, CalendarOutlined, TrophyOutlined, LogoutOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";

const { Title, Text } = Typography;

export const ProfilePage = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState<{
    username: string;
    email: string;
    registeredAt?: string;
    amountOfHats?: number;
  } | null>(null);

  useEffect(() => {
    const token = localStorage.getItem("authToken");
    const username = localStorage.getItem("username");
    const email = localStorage.getItem("email");

    if (!token) {
      navigate("/login");
      return;
    }

    setUser({
      username: username || "Captain",
      email: email || "captain@pirat.tv",
      registeredAt: "September 2026",
      amountOfHats: 5,
    });
    setLoading(false);
  }, [navigate]);

  const handleLogout = () => {
    localStorage.removeItem("authToken");
    localStorage.removeItem("username");
    localStorage.removeItem("email");
    localStorage.removeItem("rememberMe");
    navigate("/login");
  };

  if (loading) {
    return (
      <div style={{ textAlign: "center", padding: "100px 0", backgroundColor: "#141414", minHeight: "100vh" }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div style={{ maxWidth: "800px", margin: "0 auto", padding: "40px 16px" }}>
      <div style={{ marginBottom: "24px" }}>
        <Title level={2} style={{ color: "#fff", marginBottom: "4px" }}>
          Captain's Cabin
        </Title>
        <Text style={{ color: "#999" }}>Manage your account settings and pirate stats</Text>
      </div>

      <Card
        style={{
          backgroundColor: "#1f1f1f",
          border: "none",
          borderRadius: "16px",
          boxShadow: "0 10px 30px rgba(0,0,0,0.5)",
        }}
        bodyStyle={{ padding: "32px" }}
      >
        <Row gutter={[32, 32]} align="middle">
          <Col xs={24} sm={8} style={{ textAlign: "center" }}>
            <Avatar
              size={110}
              icon={<UserOutlined />}
              style={{
                backgroundColor: "#333",
                color: "#E50914",
                border: "3px solid #E50914",
                marginBottom: "16px",
              }}
            />
            <Title level={4} style={{ color: "#fff", margin: 0 }}>
              {user?.username}
            </Title>
          </Col>

          <Col xs={24} sm={16}>
            <div style={{ display: "flex", flexDirection: "column", gap: "16px" }}>
              <div style={{ display: "flex", alignItems: "center", gap: "12px" }}>
                <MailOutlined style={{ fontSize: "18px", color: "#E50914" }} />
                <div>
                  <Text style={{ color: "#999", fontSize: "12px", display: "block" }}>Email</Text>
                  <Text style={{ color: "#fff", fontSize: "15px" }}>{user?.email}</Text>
                </div>
              </div>

              <div style={{ display: "flex", alignItems: "center", gap: "12px" }}>
                <CalendarOutlined style={{ fontSize: "18px", color: "#E50914" }} />
                <div>
                  <Text style={{ color: "#999", fontSize: "12px", display: "block" }}>Member Since</Text>
                  <Text style={{ color: "#fff", fontSize: "15px" }}>{user?.registeredAt}</Text>
                </div>
              </div>
            </div>
          </Col>
        </Row>

        <Divider style={{ borderColor: "#333", margin: "32px 0" }} />

        <Row gutter={16} style={{ marginBottom: "32px" }}>
          <Col span={12}>
            <Card style={{ backgroundColor: "#141414", border: "1px solid #333", borderRadius: "12px" }} bodyStyle={{ padding: "16px" }}>
              <Statistic
                title={<span style={{ color: "#999" }}>Amount of Hats</span>}
                value={user?.amountOfHats}
                valueStyle={{ color: "#E50914", fontWeight: "bold" }}
                prefix={<TrophyOutlined />}
              />
            </Card>
          </Col>
          <Col span={12}>
            <Card style={{ backgroundColor: "#141414", border: "1px solid #333", borderRadius: "12px" }} bodyStyle={{ padding: "16px" }}>
              <Statistic
                title={<span style={{ color: "#999" }}>Account Status</span>}
                value="Active"
                valueStyle={{ color: "#52c41a", fontSize: "20px", fontWeight: "bold" }}
              />
            </Card>
          </Col>
        </Row>

        <div style={{ display: "flex", justifyContent: "flex-end" }}>
          <Button
            type="primary"
            danger
            icon={<LogoutOutlined />}
            onClick={handleLogout}
            style={{
              height: "40px",
              fontWeight: 600,
              backgroundColor: "#E50914",
              borderColor: "#E50914",
            }}
          >
            Log Out
          </Button>
        </div>
      </Card>
    </div>
  );
};