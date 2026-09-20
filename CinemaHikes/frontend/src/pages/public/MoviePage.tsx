import { useParams, useNavigate } from "react-router-dom";
import { Typography, Row, Col, Button, Tag, Divider } from "antd";
import {
  ArrowLeftOutlined,
  PlayCircleOutlined,
  StarFilled,
} from "@ant-design/icons";

const { Title, Text, Paragraph } = Typography;

const MOCK_MOVIE_DETAILS = {
  id: 1,
  ruTitle: "Pirates of the Caribbean: The Curse of the Black Pearl",
  ruInEngTitle: "Pirates of the Caribbean",
  releaseYear: 2003,
  kpRating: 8.3,
  posterUrl: "https://via.placeholder.com/400x600/222/E50914?text=Pirates",
  description:
    "The swashbuckling tale of captain Jack Sparrow, a charismatic pirate whose life of adventure is turned upside down when his wicked foe, Captain Barbossa, steals his ship the Black Pearl and later attacks Port Royal...",
  genres: ["Adventure", "Fantasy", "Action"],
  duration: "143 min.",
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
        onClick={() => navigate("/")}
        style={{ marginBottom: "20px", paddingLeft: 0, color: "#999" }}
      >
        Back to catalog
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
            Watch Movie
          </Button>
        </Col>
      </Row>

      <Divider style={{ borderColor: "#333", margin: "48px 0" }} />

      <div>
        <Title level={3} style={{ color: "#fff", marginBottom: "24px" }}>
          Viewer Reviews
        </Title>
        <Text style={{ color: "#666" }}>
          No reviews yet. Be the first captain to leave a review!
        </Text>
      </div>
    </div>
  );
};