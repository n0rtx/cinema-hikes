import { useState } from "react";
import { useParams } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  Typography,
  Tag,
  Spin,
  Empty,
  Divider,
  Input,
  Rate,
  Button,
  message,
} from "antd";
import { StarFilled } from "@ant-design/icons";
import { fetchMovieDetails } from "../../features/movie-details/api/fetchMovieDetails";
import { createReview } from "../../features/movie-details/api/createReview";
import { VideoPlayer } from "../../features/movie-details/components/VideoPlayer";
import { ReviewStatus } from "../../shared/types/Enums/ReviewStatus";

const { Title, Text, Paragraph } = Typography;
const { TextArea } = Input;

const metaLabel: React.CSSProperties = {
  color: "#888",
  minWidth: 110,
  display: "inline-block",
  flexShrink: 0,
};

const metaRow: React.CSSProperties = {
  display: "flex",
  gap: 12,
  marginBottom: 8,
  fontSize: 14,
  lineHeight: 1.5,
};

export const MoviePage = () => {
  const { id } = useParams<{ id: string }>();
  const queryClient = useQueryClient();

  const [rating, setRating] = useState(0);
  const [comment, setComment] = useState("");

  const { data: movie, isLoading, isError } = useQuery({
    queryKey: ["movie", id],
    queryFn: () => fetchMovieDetails(Number(id)),
    enabled: !!id && !Number.isNaN(Number(id)),
  });

  const mutation = useMutation({
    mutationFn: createReview,
    onSuccess: () => {
      message.success("Отзыв отправлен на модерацию");
      setRating(0);
      setComment("");
      queryClient.invalidateQueries({ queryKey: ["movie", id] });
    },
    onError: () => {
      message.error("Не удалось отправить отзыв. Войдите в аккаунт и попробуйте снова.");
    },
  });

  if (isLoading) {
    return (
      <div style={{ display: "flex", justifyContent: "center", padding: "80px 0" }}>
        <Spin size="large" />
      </div>
    );
  }

  if (isError || !movie) {
    return (
      <Empty
        description={<Text style={{ color: "#888" }}>Фильм не найден</Text>}
        style={{ padding: "80px 0" }}
      />
    );
  }

  const title = movie.ruTitle || movie.uaTitle || "Без названия";
  const description = movie.description || "";
  const director = movie.director || "—";
  const year = movie.releaseYear || "—";
  const poster = movie.posterUrl || "";
  const kpRating = movie.kpRating ?? 0;
  const genres = movie.genres ?? [];
  const movieLinks = movie.movieLinks ?? [];
  const videoSources = movie.videoSources ?? [];
  const reviews = (movie.reviews ?? []).filter(
    (r) => r.status === ReviewStatus.Approved || r.status === undefined
  );

  const handleSubmit = () => {
    if (rating < 1) {
      message.warning("Поставьте оценку от 1 до 5");
      return;
    }
    if (!comment.trim()) {
      message.warning("Напишите комментарий");
      return;
    }
    mutation.mutate({
      movieId: movie.id,
      text: comment.trim(),
      rating,
    });
  };

  return (
    <div style={{ maxWidth: 1100, margin: "0 auto", padding: "0 0 48px" }}>
      {/* TITLE */}
      <div style={{ marginBottom: 20 }}>
        <Title
          level={2}
          style={{
            color: "#fff",
            margin: 0,
            fontWeight: 700,
            fontSize: 28,
            lineHeight: 1.25,
          }}
        >
          {title}
          {year !== "—" && (
            <Text style={{ color: "#888", fontWeight: 400, fontSize: 22 }}>
              {" "}
              ({year})
            </Text>
          )}
        </Title>
      </div>

      {/* INFO: poster + meta */}
      <div
        style={{
          display: "flex",
          gap: 24,
          marginBottom: 28,
          flexWrap: "wrap",
        }}
      >
        <div style={{ flexShrink: 0 }}>
          {poster ? (
            <img
              src={poster}
              alt={title}
              style={{
                width: 220,
                height: 330,
                objectFit: "cover",
                borderRadius: 6,
                border: "1px solid #2a2a2a",
                backgroundColor: "#1a1a1a",
                display: "block",
              }}
            />
          ) : (
            <div
              style={{
                width: 220,
                height: 330,
                borderRadius: 6,
                backgroundColor: "#1a1a1a",
                border: "1px solid #2a2a2a",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              <Text style={{ color: "#555" }}>Нет постера</Text>
            </div>
          )}
        </div>

        <div style={{ flex: 1, minWidth: 260 }}>
          <div style={metaRow}>
            <span style={metaLabel}>Рейтинг КП</span>
            <span style={{ color: "#E50914", fontWeight: 700, fontSize: 16 }}>
              <StarFilled style={{ marginRight: 4 }} />
              {Number(kpRating).toFixed(1)}
            </span>
          </div>

          <div style={metaRow}>
            <span style={metaLabel}>Год</span>
            <span style={{ color: "#ddd" }}>{year}</span>
          </div>

          <div style={metaRow}>
            <span style={metaLabel}>Режиссёр</span>
            <span style={{ color: "#ddd" }}>{director}</span>
          </div>

          {genres.length > 0 && (
            <div style={metaRow}>
              <span style={metaLabel}>Жанр</span>
              <span style={{ display: "flex", flexWrap: "wrap", gap: 6 }}>
                {genres.map((g) => (
                  <Tag
                    key={g.id}
                    style={{
                      margin: 0,
                      backgroundColor: "#1f1f1f",
                      color: "#ccc",
                      border: "1px solid #333",
                      borderRadius: 3,
                    }}
                  >
                    {g.name}
                  </Tag>
                ))}
              </span>
            </div>
          )}

          <Divider style={{ borderColor: "#2a2a2a", margin: "14px 0" }} />

          <Paragraph
            style={{
              color: "#bbb",
              fontSize: 14,
              lineHeight: 1.65,
              margin: 0,
              maxHeight: 180,
              overflow: "hidden",
            }}
          >
            {description || "Описание пока отсутствует."}
          </Paragraph>
        </div>
      </div>

      {/* PLAYER */}
      <div style={{ marginBottom: 32 }}>
        <Title
          level={4}
          style={{
            color: "#fff",
            marginBottom: 12,
            fontWeight: 600,
            fontSize: 18,
          }}
        >
          Смотреть онлайн
        </Title>
        <VideoPlayer
          movieLinks={movieLinks}
          videoSources={videoSources}
          poster={poster}
        />
      </div>

      {/* DESCRIPTION */}
      {description && (
        <div
          style={{
            backgroundColor: "#1a1a1a",
            border: "1px solid #2a2a2a",
            borderRadius: 8,
            padding: "20px 24px",
            marginBottom: 32,
          }}
        >
          <Title level={5} style={{ color: "#fff", marginTop: 0, marginBottom: 12 }}>
            Описание
          </Title>
          <Paragraph style={{ color: "#ccc", fontSize: 15, lineHeight: 1.7, margin: 0 }}>
            {description}
          </Paragraph>
        </div>
      )}

      {/* ===== REVIEWS ===== */}
      <div
        style={{
          backgroundColor: "#1a1a1a",
          border: "1px solid #2a2a2a",
          borderRadius: 8,
          padding: "20px 24px",
        }}
      >
        <Title level={5} style={{ color: "#fff", marginTop: 0, marginBottom: 16 }}>
          Отзывы и оценки
        </Title>

        {/* Form */}
        <div
          style={{
            backgroundColor: "#141414",
            border: "1px solid #2a2a2a",
            borderRadius: 6,
            padding: 16,
            marginBottom: 24,
          }}
        >
          <Text style={{ color: "#aaa", display: "block", marginBottom: 8 }}>
            Ваша оценка
          </Text>
          <Rate
            value={rating}
            onChange={setRating}
            style={{ color: "#E50914", fontSize: 28, marginBottom: 16 }}
          />

          <Text style={{ color: "#aaa", display: "block", marginBottom: 8 }}>
            Комментарий
          </Text>
          <TextArea
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Напишите, что думаете о фильме..."
            rows={4}
            maxLength={2000}
            showCount
            style={{
              backgroundColor: "#1a1a1a",
              borderColor: "#333",
              color: "#fff",
              marginBottom: 16,
            }}
          />

          <Button
            type="primary"
            onClick={handleSubmit}
            loading={mutation.isPending}
            style={{
              backgroundColor: "#E50914",
              borderColor: "#E50914",
              fontWeight: 600,
            }}
          >
            Отправить отзыв
          </Button>
        </div>

        {/* List */}
        {reviews.length === 0 ? (
          <Empty
            description={
              <Text style={{ color: "#666" }}>Пока нет отзывов. Будьте первым!</Text>
            }
            image={Empty.PRESENTED_IMAGE_SIMPLE}
          />
        ) : (
          <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
            {reviews.map((r) => (
              <div
                key={r.id}
                style={{
                  borderTop: "1px solid #2a2a2a",
                  paddingTop: 14,
                }}
              >
                <div
                  style={{
                    display: "flex",
                    alignItems: "center",
                    gap: 12,
                    marginBottom: 8,
                  }}
                >
                  <Rate
                    disabled
                    value={Math.round(r.rating)}
                    style={{ color: "#E50914", fontSize: 16 }}
                  />
                  <Text style={{ color: "#666", fontSize: 12 }}>
                    {r.createdAt
                      ? new Date(r.createdAt).toLocaleDateString("ru-RU")
                      : ""}
                  </Text>
                </div>
                <Paragraph style={{ color: "#ccc", margin: 0, fontSize: 14 }}>
                  {r.text}
                </Paragraph>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};