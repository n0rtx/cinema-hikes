import { Typography, Row, Col, Card, Button } from "antd";
import {
  SendOutlined,
  QuestionCircleOutlined,
  CustomerServiceOutlined,
} from "@ant-design/icons";
import { useTranslation } from "react-i18next";

const { Title, Paragraph, Text } = Typography;

export const SupportPage = () => {
  const { t } = useTranslation();

  return (
    <div style={{ maxWidth: "900px", margin: "0 auto", paddingBottom: "40px" }}>
      <div style={{ marginBottom: "32px", textAlign: "center" }}>
        <Title level={2} style={{ color: "#fff", marginBottom: "8px" }}>
          {t("support.title")}
        </Title>
        <Text style={{ color: "#999", fontSize: "16px" }}>
          {t("support.subtitle")}
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
                {t("support.telegramTitle")}
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
              {t("support.telegramDesc")}
            </Paragraph>
            <Button
              type="primary"
              icon={<SendOutlined />}
              onClick={() =>
                window.open(
                  "https://t.me/cinema_hikes_downloader_bot",
                  "_blank",
                )
              }
              style={{
                backgroundColor: "#0088cc",
                borderColor: "#0088cc",
                fontWeight: 600,
                height: "40px",
                cursor: "pointer",
              }}
            >
              {t("support.openTelegram")}
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
                {t("support.helpTitle")}
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
              {t("support.helpDesc")}
            </Paragraph>
            <Button
              type="primary"
              onClick={() =>
                (window.location.href =
                  "mailto:pirattvteam@gmail.com?subject=PIRAT.tv%20Support%20Request")
              }
              style={{
                backgroundColor: "#E50914",
                borderColor: "#E50914",
                fontWeight: 600,
                height: "40px",
                cursor: "pointer",
              }}
            >
              {t("support.contactSupport")}
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
            {t("support.faqTitle")}
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
              {t("support.faq1q")}
            </Text>
            <Text style={{ color: "#aaa", fontSize: "14px" }}>
              {t("support.faq1a")}
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
              {t("support.faq2q")}
            </Text>
            <Text style={{ color: "#aaa", fontSize: "14px" }}>
              {t("support.faq2a")}
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
              {t("support.faq3q")}
            </Text>
            <Text style={{ color: "#aaa", fontSize: "14px" }}>
              {t("support.faq3a")}
            </Text>
          </div>
        </div>
      </Card>
    </div>
  );
};
