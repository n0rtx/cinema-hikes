import { Typography, Button } from "antd";
import { CompassOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

const { Title, Paragraph } = Typography;

export const NotFoundPage = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();

  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "60vh",
        textAlign: "center",
      }}
    >
      <CompassOutlined
        style={{ fontSize: "80px", color: "#E50914", marginBottom: "24px" }}
      />
      <Title level={1} style={{ fontSize: "80px", margin: "0" }}>
        404
      </Title>
      <Title level={3} style={{ marginTop: "8px" }}>
        {t("notFound.title")}
      </Title>
      <Paragraph
        style={{
          fontSize: "18px",
          color: "#999",
          maxWidth: "400px",
          marginTop: "16px",
        }}
      >
        {t("notFound.text")}
      </Paragraph>
      <Button
        type="primary"
        size="large"
        onClick={() => navigate("/")}
        style={{
          marginTop: "24px",
          backgroundColor: "#E50914",
          borderColor: "#E50914",
        }}
      >
        {t("notFound.button")}
      </Button>
    </div>
  );
};
