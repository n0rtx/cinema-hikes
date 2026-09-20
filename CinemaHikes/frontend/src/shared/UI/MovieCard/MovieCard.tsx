import { Card, Typography, Tag } from "antd";
import { StarFilled } from "@ant-design/icons";
import type { MovieListItem } from "../../types/DTO/MoveListItemDto";
const { Title, Text } = Typography;
interface MovieCardProps {
  movie: MovieListItem;
}
export const MovieCard = ({ movie }: MovieCardProps) => {
  const displayTitle = movie.ruInEngTitle || movie.ruTitle;
  return (
    <Card
      hoverable
      style={{
        width: "100%",
        backgroundColor: "#1f1f1f",
        border: "none",
        borderRadius: "12px",
        overflow: "hidden",
      }}
      bodyStyle={{ padding: "16px" }}
      cover={
        <div style={{ position: "relative", paddingTop: "150%" }}>
          <img
            alt={displayTitle}
            src={movie.posterUrl}
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              width: "100%",
              height: "100%",
              objectFit: "cover",
            }}
          />
          <Tag
            color="#E50914"
            style={{
              position: "absolute",
              top: "12px",
              right: "8px",
              margin: 0,
              fontWeight: "bold",
              borderRadius: "4px",
              border: "none",
            }}
          >
            <StarFilled style={{ marginRight: "4px" }} />
            {movie.kpRating.toFixed(1)}
          </Tag>
        </div>
      }
    >
      <Title
        level={5}
        style={{
          color: "#fff",
          margin: "0 0 4px 0",
          whiteSpace: "nowrap",
          overflow: "hidden",
          textOverflow: "ellipsis",
        }}
        title={displayTitle}
      >
        {displayTitle}
      </Title>
      <Text style={{ color: "#888" }}>{movie.releaseYear}</Text>
    </Card>
  );
};
