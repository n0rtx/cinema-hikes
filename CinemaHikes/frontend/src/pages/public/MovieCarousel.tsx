import { Typography, Row, Col, Alert } from "antd";
import { MovieCard } from "../../shared/UI/MovieCard/MovieCard";
import { useMoviesQuery } from "../../features/catalog/hooks/useMoviesQuery";

const { Title, Text } = Typography;

interface MovieCarouselProps {
  sectionTitle: string;
  minRating?: number;
  pageSize?: number;
}

const LoaderCircle = () => (
  <div
    style={{
      width: 40,
      height: 40,
      borderRadius: "50%",
      border: "3px solid #444",
      borderTopColor: "#E50914",
      animation: "spin 0.8s linear infinite",
    }}
  />
);

export const MovieCarousel = ({
  sectionTitle,
  minRating,
  pageSize = 10,
}: MovieCarouselProps) => {
  const { data: movies = [], isLoading, isError, error } = useMoviesQuery({
    minRating,
    pageSize,
  });

  return (
    <div style={{ marginBottom: "48px" }}>
      <style>
        {`@keyframes spin { to { transform: rotate(360deg); } }`}
      </style>

      <Title
        level={3}
        style={{
          color: "#fff",
          marginBottom: "24px",
          borderLeft: "4px solid #E50914",
          paddingLeft: "12px",
        }}
      >
        {sectionTitle}
      </Title>

      {isLoading && (
        <div
          style={{
            display: "flex",
            justifyContent: "center",
            padding: "32px 0",
          }}
        >
          <LoaderCircle />
        </div>
      )}

      {isError && (
        <Alert
          type="error"
          message="Не удалось загрузить фильмы"
          description={error instanceof Error ? error.message : "Ошибка сети"}
          showIcon
          style={{ marginBottom: 16 }}
        />
      )}

      {!isLoading && !isError && movies.length === 0 && (
        <Text style={{ color: "#888" }}>В этой категории пока нет фильмов</Text>
      )}

      {!isLoading && !isError && movies.length > 0 && (
        <Row gutter={[24, 32]}>
          {movies.map((movie) => (
            <Col xs={12} sm={8} md={6} lg={4} key={movie.id}>
              <MovieCard movie={movie} />
            </Col>
          ))}
        </Row>
      )}
    </div>
  );
};