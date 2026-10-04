import { Typography, Row, Col, Card, Button } from "antd";
import {
  SendOutlined,
  QuestionCircleOutlined,
  CustomerServiceOutlined,
} from "@ant-design/icons";

const { Title, Paragraph, Text } = Typography;

export const SupportPage = () => {
  return (
    <div style={{ maxWidth: "900px", margin: "0 auto", paddingBottom: "40px" }}>
      <div style={{ marginBottom: "32px", textAlign: "center" }}>
        <Title level={2} style={{ color: "#fff", marginBottom: "8px" }}>
          Captain's Support
        </Title>
        <Text style={{ color: "#999", fontSize: "16px" }}>
          Need help navigating or downloading cinematic treasures? We've got
          your back.
        </Text>
      </div>

      <Row gutter={[24, 24]}>
        <Col xs={24} md={12}>
          <Card
            style={{
              backgroundColor: "#1f1f1f",
              border: "none",
              borderRadius: "16px",
              height: "100%",
            }}
            bodyStyle={{ padding: "24px" }}
          >
            <div
              style={{
                display: "flex",
                alignItems: "center",
                gap: "12px",
                marginBottom: "16px",
              }}
            >
              <SendOutlined style={{ fontSize: "28px", color: "#0088cc" }} />
              <Title level={4} style={{ color: "#fff", margin: 0 }}>
                Telegram Bot
              </Title>
            </div>
            <Paragraph
              style={{
                color: "#aaa",
                fontSize: "15px",
                lineHeight: "1.5",
                marginBottom: "24px",
              }}
            >
              Use our official Telegram bot to search for movies, choose video
              quality, and download or stream media seamlessly.
            </Paragraph>
            <Button
              type="primary"
              icon={<SendOutlined />}
              onClick={() => window.open("https://t.me/cinema_hikes_downloader_bot", "_blank")}
              style={{
                backgroundColor: "#0088cc",
                borderColor: "#0088cc",
                fontWeight: 600,
                height: "40px",
                cursor: "pointer",
              }}
            >
              Open Telegram Bot
            </Button>
          </Card>
        </Col>

        <Col xs={24} md={12}>
          <Card
            style={{
              backgroundColor: "#1f1f1f",
              border: "none",
              borderRadius: "16px",
              height: "100%",
            }}
            bodyStyle={{ padding: "24px" }}
          >
            <div
              style={{
                display: "flex",
                alignItems: "center",
                gap: "12px",
                marginBottom: "16px",
              }}
            >
              <CustomerServiceOutlined
                style={{ fontSize: "28px", color: "#E50914" }}
              />
              <Title level={4} style={{ color: "#fff", margin: 0 }}>
                Crew Assistance
              </Title>
            </div>
            <Paragraph
              style={{
                color: "#aaa",
                fontSize: "15px",
                lineHeight: "1.5",
                marginBottom: "24px",
              }}
            >
              Encountered a broken link, parsing error, or missing video source?
              Reach out to the administrators directly.
            </Paragraph>
            <Button
              type="primary"
              onClick={() => window.location.href = "mailto:pirattvteam@gmail.com?subject=PIRAT.tv%20Support%20Request"}
              style={{
                backgroundColor: "#E50914",
                borderColor: "#E50914",
                fontWeight: 600,
                height: "40px",
                cursor: "pointer",
              }}
            >
              Contact Support
            </Button>
          </Card>
        </Col>
      </Row>

      <Card
        style={{
          backgroundColor: "#1f1f1f",
          border: "none",
          borderRadius: "16px",
          marginTop: "24px",
        }}
        bodyStyle={{ padding: "24px" }}
      >
        <div
          style={{
            display: "flex",
            alignItems: "center",
            gap: "12px",
            marginBottom: "16px",
          }}
        >
          <QuestionCircleOutlined
            style={{ fontSize: "24px", color: "#faad14" }}
          />
          <Title level={4} style={{ color: "#fff", margin: 0 }}>
            Frequently Asked Questions
          </Title>
        </div>

        <div style={{ display: "flex", flexDirection: "column", gap: "16px" }}>
          <div>
            <Text
              style={{
                color: "#fff",
                fontWeight: 600,
                display: "block",
                marginBottom: "4px",
              }}
            >
              How do I download movies using the bot?
            </Text>
            <Text style={{ color: "#aaa", fontSize: "14px" }}>
              Send a movie title to the Telegram bot, select your desired
              translation studio, and choose the video quality (up to 1080p).
            </Text>
          </div>

          <div>
            <Text
              style={{
                color: "#fff",
                fontWeight: 600,
                display: "block",
                marginBottom: "4px",
              }}
            >
              Are all movies completely free?
            </Text>
            <Text style={{ color: "#aaa", fontSize: "14px" }}>
              Yes, PIRAT.tv provides open access to the cinematic catalog for
              all registered captains without hidden fees.
            </Text>
          </div>
        </div>
      </Card>
    </div>
  );
};