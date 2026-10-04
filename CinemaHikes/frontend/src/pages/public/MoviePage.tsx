<<<<<<< Updated upstream
=======
import { useParams, useNavigate } from "react-router-dom";
import { Typography, Row, Col, Button, Tag, Divider, Spin } from "antd";
import {
  ArrowLeftOutlined,
  PlayCircleOutlined,
  StarFilled,
} from "@ant-design/icons";
import { useQuery } from "@tanstack/react-query";
import { axiosClient } from "../../shared/api/axiosClient";
import type { MovieDetailsDto } from "../../shared/types/DTO/CatalogDtos/MovieDetailsDto";

const { Title, Text, Paragraph } = Typography;

export const MoviePage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const { data: movie, isLoading, isError } = useQuery<MovieDetailsDto>({
    queryKey: ["movie", id],
    queryFn: async () => {
      const response = await axiosClient.get(`/movies/${id}`);
      return response.data;
    },
    enabled: !!id,
  });

  if (isLoading) {
    return (
      <div style={{ textAlign: "center", padding: "150px 0", minHeight: "60vh" }}>
        <Spin size="large" />
      </div>
    );
  }

  if (isError || !movie) {
    return (
      <div style={{ textAlign: "center", padding: "150px 0", minHeight: "60vh" }}>
        <Title level={3} style={{ color: "#E50914" }}>
          Oops! Movie not found.
        </Title>
        <Button
          type="primary"
          onClick={() => navigate("/catalog")}
          style={{ backgroundColor: "#E50914", borderColor: "#E50914" }}
        >
          Back to catalog
        </Button>
      </div>
    );
  }

  return (
    <div
      style={{ maxWidth: "1200px", margin: "0 auto", paddingBottom: "40px", padding: "0 16px" }}
    >
      <Button
        type="link"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate(-1)}
        style={{ marginBottom: "20px", paddingLeft: 0, color: "#999" }}
      >
        Back
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
              src={movie.posterUrl || "https://via.placeholder.com/400x600/222/E50914?text=No+Poster"}
              alt={movie.ruTitle}
              style={{ width: "100%", display: "block" }}
            />
          </div>
        </Col>

        <Col xs={24} md={16}>
          <Title level={2} style={{ color: "#fff", marginBottom: "8px" }}>
            {movie.ruTitle}
          </Title>
          
          {(movie.ruInEngTitle || movie.releaseYear) && (
            <Text
              type="secondary"
              style={{ fontSize: "16px", display: "block", marginBottom: "16px" }}
            >
              {movie.ruInEngTitle} {movie.releaseYear ? `(${movie.releaseYear})` : ""}
            </Text>
          )}

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
              {movie.kpRating ? movie.kpRating.toFixed(1) : "N/A"} KP
            </Tag>
          </div>

          <div style={{ display: "flex", gap: "8px", marginBottom: "24px", flexWrap: "wrap" }}>
            {movie.genres?.map((genre: any, idx: number) => (
              <Tag
                key={idx}
                style={{
                  backgroundColor: "#222",
                  color: "#ccc",
                  border: "1px solid #444",
                }}
              >
                {typeof genre === 'string' ? genre : genre.name}
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
            Movie description is currently unavailable.
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

      <div style={{ marginBottom: "48px" }}>
        <Title level={3} style={{ color: "#fff", marginBottom: "24px", borderLeft: "4px solid #E50914", paddingLeft: "12px" }}>
          Player
        </Title>
        <div 
          style={{ 
            width: "100%", 
            aspectRatio: "16/9", 
            backgroundColor: "#111", 
            border: "1px solid #333",
            borderRadius: "12px",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            color: "#666"
          }}
        >
          Video source will be loaded here
        </div>
      </div>

      <div>
        <Title level={3} style={{ color: "#fff", marginBottom: "24px", borderLeft: "4px solid #E50914", paddingLeft: "12px" }}>
          Viewer Reviews
        </Title>
        <Text style={{ color: "#666" }}>
          No reviews yet. Be the first captain to leave a review!
        </Text>
      </div>
    </div>
  );
};
>>>>>>> Stashed changes
