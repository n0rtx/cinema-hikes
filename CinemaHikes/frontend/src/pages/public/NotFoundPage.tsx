import { Typography, Button } from "antd";
import { CompassOutlined } from "@ant-design/icons";

const { Title, Paragraph } = Typography;

export const NotFoundPage = () => {
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
        Мёртвые воды
      </Title>
      <Paragraph
        style={{
          fontSize: "18px",
          color: "#999",
          maxWidth: "400px",
          marginTop: "16px",
        }}
      >
        Капитан, кажется, мы наткнулись на риф! Эти координаты никуда не ведут.
        Давайте развернём корабль, пока не затонули.
      </Paragraph>
      <Button
        type="primary"
        size="large"
        href="/"
        style={{
          marginTop: "24px",
          backgroundColor: "#E50914",
          borderColor: "#E50914",
        }}
      >
        В безопасную гавань
      </Button>
    </div>
  );
};