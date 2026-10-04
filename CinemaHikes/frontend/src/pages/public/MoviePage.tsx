<<<<<<< HEAD
import {  useNavigate } from "react-router-dom";
import { Typography, Row, Col, Button, Tag, Divider } from "antd";
=======
<<<<<<< Updated upstream
=======
import { useParams, useNavigate } from "react-router-dom";
import { Typography, Row, Col, Button, Tag, Divider, Spin } from "antd";
>>>>>>> 8222787 (removed some pages)
import {
  ArrowLeftOutlined,
  PlayCircleOutlined,
  StarFilled,
} from "@ant-design/icons";
<<<<<<< HEAD

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
//   const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  return (
    <div
      style={{ maxWidth: "1200px", margin: "0 auto", paddingBottom: "40px" }}
=======
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
>>>>>>> 8222787 (removed some pages)
    >
      <Button
        type="link"
        icon={<ArrowLeftOutlined />}
<<<<<<< HEAD
        onClick={() => navigate("/")}
        style={{ marginBottom: "20px", paddingLeft: 0, color: "#999" }}
      >
        Back to catalog
=======
        onClick={() => navigate(-1)}
        style={{ marginBottom: "20px", paddingLeft: 0, color: "#999" }}
      >
        Back
>>>>>>> 8222787 (removed some pages)
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
<<<<<<< HEAD
              src={MOCK_MOVIE_DETAILS.posterUrl}
              alt={MOCK_MOVIE_DETAILS.ruTitle}
=======
              src={movie.posterUrl || "https://via.placeholder.com/400x600/222/E50914?text=No+Poster"}
              alt={movie.ruTitle}
>>>>>>> 8222787 (removed some pages)
              style={{ width: "100%", display: "block" }}
            />
          </div>
        </Col>

        <Col xs={24} md={16}>
          <Title level={2} style={{ color: "#fff", marginBottom: "8px" }}>
<<<<<<< HEAD
            {MOCK_MOVIE_DETAILS.ruTitle}
          </Title>
          <Text
            type="secondary"
            style={{ fontSize: "16px", display: "block", marginBottom: "16px" }}
          >
            {MOCK_MOVIE_DETAILS.ruInEngTitle} ({MOCK_MOVIE_DETAILS.releaseYear})
          </Text>
=======
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
>>>>>>> 8222787 (removed some pages)

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
<<<<<<< HEAD
              {MOCK_MOVIE_DETAILS.kpRating} KP
            </Tag>
            <Text style={{ color: "#888" }}>{MOCK_MOVIE_DETAILS.duration}</Text>
          </div>

          <div style={{ display: "flex", gap: "8px", marginBottom: "24px" }}>
            {MOCK_MOVIE_DETAILS.genres.map((genre) => (
              <Tag
                key={genre}
=======
              {movie.kpRating ? movie.kpRating.toFixed(1) : "N/A"} KP
            </Tag>
          </div>

          <div style={{ display: "flex", gap: "8px", marginBottom: "24px", flexWrap: "wrap" }}>
            {movie.genres?.map((genre: any, idx: number) => (
              <Tag
                key={idx}
>>>>>>> 8222787 (removed some pages)
                style={{
                  backgroundColor: "#222",
                  color: "#ccc",
                  border: "1px solid #444",
                }}
              >
<<<<<<< HEAD
                {genre}
=======
                {typeof genre === 'string' ? genre : genre.name}
>>>>>>> 8222787 (removed some pages)
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
<<<<<<< HEAD
            {MOCK_MOVIE_DETAILS.description}
=======
            Movie description is currently unavailable.
>>>>>>> 8222787 (removed some pages)
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

<<<<<<< HEAD
      <div>
        <Title level={3} style={{ color: "#fff", marginBottom: "24px" }}>
=======
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
>>>>>>> 8222787 (removed some pages)
          Viewer Reviews
        </Title>
        <Text style={{ color: "#666" }}>
          No reviews yet. Be the first captain to leave a review!
        </Text>
      </div>
    </div>
  );
<<<<<<< HEAD
};
=======
};
>>>>>>> Stashed changes
>>>>>>> 8222787 (removed some pages)
