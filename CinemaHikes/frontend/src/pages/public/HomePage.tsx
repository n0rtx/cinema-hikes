import { Typography, Button } from "antd";
import { MovieCarousel } from "./MovieCarousel";
import { PlayCircleOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import LogoPiratTv from "../../../public/assets/logos/LogoPiratTv.jpg";

const { Title, Text } = Typography;

export const HomePage = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();

  return (
    <div>
      <div
        style={{
          position: "relative",
          width: "100%",
          minHeight: "420px",
          backgroundColor: "#141414",
          backgroundImage: `
    linear-gradient(90deg, #141414 0%, #141414 40%, rgba(20, 20, 20, 0.8) 65%, rgba(20, 20, 20, 0.2) 100%),url("${LogoPiratTv}")`,
          backgroundSize: "contain",
          backgroundPosition: "right center",
          backgroundRepeat: "no-repeat",
          borderRadius: "16px",
          marginBottom: "48px",
          display: "flex",
          alignItems: "center",
          padding: "48px",
        }}
      >
        <div style={{ maxWidth: "600px" }}>
          <Title
            style={{
              color: "#fff",
              fontSize: "48px",
              fontWeight: 900,
              margin: 0,
            }}
          >
            {t("home.heroTitle")}
          </Title>
          <Text
            style={{
              color: "#aaa",
              fontSize: "18px",
              display: "block",
              margin: "16px 0 32px 0",
            }}
          >
            {t("home.heroSubtitle")}
          </Text>
          <Button
            type="primary"
            size="large"
            icon={<PlayCircleOutlined />}
            onClick={() => navigate("/catalog")}
            style={{
              backgroundColor: "#E50914",
              borderColor: "#E50914",
              height: "48px",
              padding: "0 32px",
              fontSize: "16px",
              fontWeight: "bold",
            }}
          >
            {t("home.watchMovies")}
          </Button>
        </div>
      </div>

      <MovieCarousel sectionTitle={t("home.topSection")} minRating={7} pageSize={12} />
    </div>
  );
};
