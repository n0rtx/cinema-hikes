import { Typography, Row, Col, Card, Avatar, Divider } from "antd";
import {
  RocketOutlined,
  CodeOutlined,
  UnlockOutlined,
  UserOutlined,
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
const { Title, Paragraph, Text } = Typography;
export const AboutPage = () => {
  const navigate = useNavigate()
  return (
    <div style={{ maxWidth: "800px", margin: "0 auto", paddingTop: "40px" }}>
      <div style={{ textAlign: "center", marginBottom: "60px" }}>
        <UnlockOutlined
          style={{ fontSize: "48px", color: "#E50914", marginBottom: "16px" }}
        />
        <Title level={1}>Restoring the Freedom to Watch</Title>
        <Paragraph
          style={{
            fontSize: "18px",
            color: "#ccc",
            lineHeight: "1.8",
            marginTop: "24px",
          }}
        >
          The internet was meant to be a free space for sharing information.
          CinemaHikes is our answer to modern restrictions. We are building a
          platform where everyone can find and enjoy their favorite movies in a
          user-friendly interface, without artificial barriers or intrusive
          limitations.
        </Paragraph>
      </div>
      <Divider style={{ borderColor: "#333" }} />
      <div style={{ marginTop: "60px" }}>
        <Title level={2} style={{ textAlign: "center", marginBottom: "40px" }}>
          Who is behind this?
        </Title>
        <Paragraph
          style={{
            textAlign: "center",
            fontSize: "16px",
            color: "#999",
            marginBottom: "40px",
          }}
        >
          This project is being developed by two dedicated students from
          <Text strong style={{ color: "#fff" }}>
            {" "}
            IT STEP Academy
          </Text>
          . We combined our programming knowledge and passion for cinema to
          bring this product to life.
        </Paragraph>
        <Row gutter={[32, 32]} justify="center">
          <Col xs={24} sm={12}>
            <Card
              style={{
                backgroundColor: "#1f1f1f",
                border: "none",
                textAlign: "center",
              }}
              bodyStyle={{ padding: "32px 24px" }}
              onClick={() => navigate('/developer/b1mq')}
            >
              <Avatar
                size={80}
                icon={<UserOutlined />}
                style={{ marginBottom: "16px" }}
                src="https://github.com/b1mq.png"
              />
              <Title level={4} style={{ margin: 0, color: "#fff" }}>
                <a
                  href="https://github.com/b1mq"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "#fff" }}
                >
                  b1mq
                </a>
              </Title>
              <Text type="secondary">Backend & Frontend (IT STEP)</Text>
              <div style={{ marginTop: "16px", color: "#E50914" }}>
                <CodeOutlined style={{ fontSize: "24px" }} />
              </div>
              <div style={{ marginTop: "16px", height: "150px", overflow: "hidden" }}>
                <img
                  src="https://camo.githubusercontent.com/ab4e1e7c0b9c43c44b78ba6714ec8bb49517c9db4b7e1a74d24ce8744acc31d9/68747470733a2f2f6d65646961302e67697068792e636f6d2f6d656469612f76312e59326c6b505463354d4749334e6a45784e4756354e6e70325a6d46784f57706c636e4e724e47566b5a5852794e6a6c6d4f474a6a4e4849784f4468326457357a646e42695a535a6c634431324d563970626e526c636d35686246396e61575a66596e6c666157516d593351395a772f4138556f3469643657436b38724a6b4a6b352f67697068792e676966"
                  alt="animation"
                  style={{ maxWidth: "100%", borderRadius: "4px" }}
                />
              </div>
            </Card>
          </Col>
          <Col xs={24} sm={12}>
            <Card
              style={{
                backgroundColor: "#1f1f1f",
                border: "none",
                textAlign: "center",

              }}
              onClick={() => navigate('/developer/n0rtx')}
              bodyStyle={{ padding: "32px 24px" }}
            >
              <Avatar
                size={80}
                icon={<UserOutlined />}
                style={{ backgroundColor: "#333", marginBottom: "16px" }}
                src="https://github.com/n0rtx.png"
              />
              <Title level={4} style={{ margin: 0, color: "#fff" }}>
                <a
                  href="https://github.com/n0rtx"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "#fff" }}
                >
                  n0rtx
                </a>
              </Title>
              <Text type="secondary">
                Backend C#,ASP.NET Core,Playwright (IT STEP)
              </Text>
              <div style={{ marginTop: "16px", color: "#E50914" }}>
                <RocketOutlined style={{ fontSize: "24px" }} />
              </div>
              <div style={{ marginTop: "16px", height: "150px", overflow: "hidden" }}>
                <img src="https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExODB2d2d0Nm5ueGJrZjloMXR5M2Y5bDIxd2wybjVuaWFxNjBweXBmNSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/Ig6ziBl0Ygs7sOtYs8/giphy.gif"
                  alt="animation"
                  style={{ maxWidth: "100%", borderRadius: "4px" }} />
              </div>
            </Card>
          </Col>
        </Row>
      </div>
    </div>
  );
};
