import { Typography, Skeleton, Button } from "antd";
import {
  PlayCircleOutlined,
  VideoCameraOutlined,
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useMoviesQuery } from "../../features/catalog/hooks/useMoviesQuery";
import { getMovieTitle } from "../../shared/utils/getMovieTitle";
import i18n from "../../shared/i18n";
import "../../features/catalog/components/CatalogPage.css";

const { Title, Text } = Typography;

interface MovieCarouselProps {
  sectionTitle: string;
  minRating?: number;
  pageSize?: number;
}

export const MovieCarousel = ({
  sectionTitle,
  minRating,
  pageSize = 10,
}: MovieCarouselProps) => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const {
    data: movies = [],
    isLoading,
    isError,
    refetch,
  } = useMoviesQuery({
    minRating,
    pageSize,
  });

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

      {isLoading && (
        <div className="cat-grid">
          {Array.from({ length: pageSize }).map((_, i) => (
            <Skeleton.Node
              key={i}
              active
              style={{
                width: "100%",
                aspectRatio: "2 / 3",
                height: "auto",
                borderRadius: 12,
              }}
            >
              {" "}
            </Skeleton.Node>
          ))}
        </div>
      )}

      {isError && (
        <div className="cat-empty">
          <Text style={{ color: "#999" }}>{t("movie.loadError")}</Text>
          <Button
            type="primary"
            onClick={() => refetch()}
            style={{ backgroundColor: "#E50914", borderColor: "#E50914" }}
          >
            {t("catalog.retry")}
          </Button>
        </div>
      )}

      {!isLoading && !isError && movies.length === 0 && (
        <Text style={{ color: "#888" }}>{t("movie.emptyCategory")}</Text>
      )}

      {!isLoading && !isError && movies.length > 0 && (
        <div className="cat-grid">
          {movies.map((m) => (
            <div
              key={m.id}
              className="cat-card"
              onClick={() => navigate(`/movies/${m.id}`)}
            >
              <div className="cat-poster">
                {m.posterUrl ? (
                  <img
                    src={m.posterUrl}
                    alt={getMovieTitle(m, i18n.language, t)}
                    loading="lazy"
                    referrerPolicy="no-referrer"
                  />
                ) : (
                  <div
                    style={{
                      height: "100%",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      fontSize: 36,
                      color: "#E50914",
                    }}
                  >
                    <VideoCameraOutlined />
                  </div>
                )}
                {m.kpRating > 0 && (
                  <span className="cat-badge">★ {m.kpRating.toFixed(1)}</span>
                )}
                <div className="cat-overlay">
                  <div className="cat-genres">
                    {(m.genres ?? [])
                      .slice(0, 3)
                      .map((g) => g.name)
                      .join(" • ")}
                  </div>
                  <span className="cat-play">
                    <PlayCircleOutlined /> {t("catalog.watch")}
                  </span>
                </div>
              </div>
              <div className="cat-title">
                {getMovieTitle(m, i18n.language, t)}
              </div>
              <div className="cat-year">{m.releaseYear || ""}</div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};