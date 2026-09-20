import { Typography,Row,Col } from "antd";
import  { MovieCard } from "../../shared/UI/MovieCard/MovieCard";
import type { MovieListItem } from "../../shared/types/DTO/MoveListItemDto";
const { Title, Text } = Typography;
const mockMovies: MovieListItem[] = [
  // МОКИ СДЕЛАНЫ С ИИ за инфу ответственности не несу
  {
    id: 1,
    ruTitle: "Пираты Карибского моря",
    ruInEngTitle: "Pirates of the Caribbean",
    releaseYear: 2003,
    kpRating: 8.3,
    posterUrl: "https://via.placeholder.com/300x450/222/E50914?text=Pirates",
  },
  {
    id: 2,
    ruTitle: "Дюна: Часть вторая",
    ruInEngTitle: "Dune: Part Two",
    releaseYear: 2024,
    kpRating: 8.8,
    posterUrl: "https://via.placeholder.com/300x450/222/E50914?text=Dune+2",
  },
  {
    id: 3,
    ruTitle: "Джентльмены",
    ruInEngTitle: "The Gentlemen",
    releaseYear: 2019,
    kpRating: 8.5,
    posterUrl: "https://via.placeholder.com/300x450/222/E50914?text=Gentlemen",
  },
  {
    id: 4,
    ruTitle: "Интерстеллар",
    ruInEngTitle: "Interstellar",
    releaseYear: 2014,
    kpRating: 8.6,
    posterUrl:
      "https://via.placeholder.com/300x450/222/E50914?text=Interstellar",
  },
];
interface MovieCarouselProps {
  sectionTitle: string;
}
export const MovieCarousel = ({ sectionTitle }: MovieCarouselProps) => {
  return (
    <div style={{ marginBottom: "48px" }}>
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

      {mockMovies.length === 0 ? (
        <Text style={{ color: "#888" }}>
          No films in this category
        </Text>
      ) : (
        <Row gutter={[24, 32]}>
          {mockMovies.map((movie) => (
            <Col xs={12} sm={8} md={6} lg={4} key={movie.id}>
              <MovieCard movie={movie} />
            </Col>
          ))}
        </Row>
      )}
    </div>
  );
};
