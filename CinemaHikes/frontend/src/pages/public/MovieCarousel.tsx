import { Typography, Row, Col } from "antd";
import { MovieCard } from "../../shared/UI/MovieCard/MovieCard";
import type { MovieListItem } from "../../shared/types/DTO/CatalogDtos/MoveListItemDto";
const { Title, Text } = Typography;

const mockMovies: MovieListItem[] = [
  {
    id: 1,
    ruTitle: "Пираты Карибского моря",
    uaTitle: "Пірати Карибського моря",
    ruInEngTitle: "Pirates of the Caribbean",
    releaseYear: 2003,
    posterUrl:
      "https://via.placeholder.com/300x450/222/E50914?text=Pirates",
    kpRating: 8.3,
    Genres: [
      {
        Id: 1,
        Name: "Приключения",
      },
      {
        Id: 2,
        Name: "Фэнтези",
      },
    ],
  },
  {
    id: 2,
    ruTitle: "Дюна: Часть вторая",
    uaTitle: "Дюна: Частина друга",
    ruInEngTitle: "Dune: Part Two",
    releaseYear: 2024,
    posterUrl:
      "https://via.placeholder.com/300x450/222/E50914?text=Dune+2",
    kpRating: 8.8,
    Genres: [
      {
        Id: 3,
        Name: "Фантастика",
      },
      {
        Id: 1,
        Name: "Приключения ",
      },
    ],
  },
  {
    id: 3,
    ruTitle: "Джентльмены",
    uaTitle: "Джентльмени",
    ruInEngTitle: "The Gentlemen",
    releaseYear: 2019,
    posterUrl:
      "https://via.placeholder.com/300x450/222/E50914?text=Gentlemen",
    kpRating: 8.5,
    Genres: [
      {
        Id: 4,
        Name: "Комедия",
      },
      {
        Id: 5,
        Name: "Криминал",
      },
    ],
  },
  {
    id: 4,
    ruTitle: "Интерстеллар",
    uaTitle: "Інтерстеллар",
    ruInEngTitle: "Interstellar",
    releaseYear: 2014,
    posterUrl:
      "https://via.placeholder.com/300x450/222/E50914?text=Interstellar",
    kpRating: 8.6,
    Genres: [
      {
        Id: 3,
        Name: "Фантастика",
      },
      {
        Id: 6,
        Name: "Драма",
      },
    ],
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
        <Text style={{ color: "#888" }}>В этой категории пока нет фильмов</Text>
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