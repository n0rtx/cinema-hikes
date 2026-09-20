import { useParams, useNavigate } from "react-router-dom";
import { Typography, Row, Col, Button, Tag, Divider, Rate } from "antd";
import {
  ArrowLeftOutlined,
  PlayCircleOutlined,
  StarFilled,
} from "@ant-design/icons";

const { Title, Text, Paragraph } = Typography;
const MOCK_MOVIE_DETAILS = {
  id: 1,
  ruTitle: "Пираты Карибского моря: Проклятие «Черной жемчужины»",
  ruInEngTitle: "Pirates of the Caribbean: The Curse of the Black Pearl",
  releaseYear: 2003,
  kpRating: 8.3,
  posterUrl: "https://via.placeholder.com/400x600/222/E50914?text=Pirates",
  description:
    "Жизнь харизматичного пирата, капитана Джека Воробья, полная увлекательных приключений, стремительно меняется, когда его заклятый враг капитан Барбосса похищает корабль Джека «Черную жемчужину», а затем нападает на Порт-Ройал...",
  genres: ["Приключения", "Фэнтези", "Боевик"],
  duration: "143 мин.",
};

export const MoviePage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  return (
    <div
      style={{ maxWidth: "1200px", margin: "0 auto", paddingBottom: "40px" }}
    >
      
      <Button
        type="link"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate(-1)}
        style={{ marginBottom: "20px", paddingLeft: 0, color: "#999" }}
      >
        Назад к каталогу
      </Button>

     
      <Row gutter={[40, 40]}>
        
        <Col xs={24} md={8}>
          <div
            style={{
              borderRadius: "12px",
              overflow: "hidden",
              boxShadow: "0 20px 40px rgba(0,0,0,0.5)",
            }}
          >
            <img
              src={MOCK_MOVIE_DETAILS.posterUrl}
              alt={MOCK_MOVIE_DETAILS.ruTitle}
              style={{ width: "100%", display: "block" }}
            />
          </div>
        </Col>

       
        <Col xs={24} md={16}>
          <Title level={2} style={{ color: "#fff", marginBottom: "8px" }}>
            {MOCK_MOVIE_DETAILS.ruTitle}
          </Title>
          <Text
            type="secondary"
            style={{ fontSize: "16px", display: "block", marginBottom: "16px" }}
          >
            {MOCK_MOVIE_DETAILS.ruInEngTitle} ({MOCK_MOVIE_DETAILS.releaseYear})
          </Text>

         
          <div
            style={{
              display: "flex",
              gap: "12px",
              alignItems: "center",
              marginBottom: "20px",
            }}
          >
            <Tag
              color="#E50914"
              style={{ fontSize: "14px", padding: "4px 8px", margin: 0 }}
            >
              <StarFilled style={{ marginRight: "4px" }} />{" "}
              {MOCK_MOVIE_DETAILS.kpRating} KP
            </Tag>
            <Text style={{ color: "#888" }}>{MOCK_MOVIE_DETAILS.duration}</Text>
          </div>

          <div style={{ display: "flex", gap: "8px", marginBottom: "24px" }}>
            {MOCK_MOVIE_DETAILS.genres.map((genre) => (
              <Tag
                key={genre}
                style={{
                  backgroundColor: "#222",
                  color: "#ccc",
                  border: "1px solid #444",
                }}
              >
                {genre}
              </Tag>
            ))}
          </div>

          <Paragraph
            style={{
              color: "#aaa",
              fontSize: "16px",
              lineHeight: "1.6",
              marginBottom: "32px",
            }}
          >
            {MOCK_MOVIE_DETAILS.description}
          </Paragraph>

        
          <Button
            type="primary"
            size="large"
            icon={<PlayCircleOutlined />}
            style={{
              backgroundColor: "#E50914",
              borderColor: "#E50914",
              height: "48px",
              padding: "0 32px",
              fontSize: "16px",
              fontWeight: "bold",
            }}
          >
            Смотреть фильм
          </Button>
        </Col>
      </Row>

      <Divider style={{ borderColor: "#333", margin: "48px 0" }} />

     
      <div>
        <Title level={3} style={{ color: "#fff", marginBottom: "24px" }}>
          Отзывы зрителей
        </Title>
        <Text style={{ color: "#666" }}>
          Пока нет отзывов. Станьте первым капитаном, оставившим рецензию!
        </Text>
      </div>
    </div>
  );
};
