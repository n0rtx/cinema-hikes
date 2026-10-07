import { useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { Typography, Tag, Spin, Row, Col, Divider, Empty } from "antd";
import { StarFilled } from "@ant-design/icons";
import { fetchMovieDetails } from "../../features/movie-details/api/fetchMovieDetails";
import { VideoPlayer } from "../../features/movie-details/components/VideoPlayer";
import type { MovieDetailsDto } from "../../shared/types/DTO/CatalogDtos/MovieDetailsDto";

const { Title, Text, Paragraph } = Typography;

export const MoviePage = () => {
    const { id } = useParams<{ id: string }>();

    const { data: movie, isLoading, isError } = useQuery({
        queryKey: ["movie", id],
        queryFn: () => fetchMovieDetails(Number(id)),
        enabled: !!id && !Number.isNaN(Number(id)),
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

    const m = movie as MovieDetailsDto & Record<string, unknown>;
    const title =
        (m.RuTitle as string) ||
        (m.ruTitle as string) ||
        (m.UaTitle as string) ||
        (m.uaTitle) ||
        "Без названия";
    const description = (m.Description as string) || (m.description as string) || "";
    const director = (m.Director as string) || (m.director as string) || "—";
    const year = (m.ReleaseYear as number) || (m.releaseYear as number) || "—";
    const poster = (m.PosterUrl as string) || (m.posterUrl as string) || "";
    const rating = (m.KpRating as number) || (m.kpRating as number) || 0;
    const genres = (m.Genres as { Id?: number; id?: number; Name?: string; name?: string }[]) || [];
    const movieLinks = (m.MovieLinks as any[]) || (m.movieLinks as any[]) || [];
    const videoSources = (m.VideoSources as any[]) || (m.videoSources as any[]) || [];

    return (
        <div>
            {/* Hero */}
            <div
                style={{
                    position: "relative",
                    width: "100%",
                    minHeight: "360px",
                    backgroundColor: "#141414",
                    backgroundImage: `linear-gradient(90deg, #141414 0%, #141414 45%, rgba(20,20,20,0.7) 70%, rgba(20,20,20,0.3) 100%), url("${poster}")`,
                    backgroundSize: "cover",
                    backgroundPosition: "center right",
                    borderRadius: "16px",
                    marginBottom: "32px",
                    display: "flex",
                    alignItems: "flex-end",
                    padding: "40px",
                }}
            >
                <div style={{ maxWidth: "640px" }}>
                    <Title
                        style={{
                            color: "#fff",
                            fontSize: "36px",
                            fontWeight: 900,
                            margin: "0 0 12px 0",
                        }}
                    >
                        {title}
                    </Title>
                    <div style={{ display: "flex", gap: "12px", alignItems: "center", flexWrap: "wrap" }}>
                        <Tag
                            color="#E50914"
                            style={{ margin: 0, fontWeight: "bold", border: "none", borderRadius: "4px" }}
                        >
                            <StarFilled style={{ marginRight: 4 }} />
                            {Number(rating).toFixed(1)}
                        </Tag>
                        <Text style={{ color: "#aaa" }}>{year}</Text>
                        <Text style={{ color: "#aaa" }}>Режиссёр: {director}</Text>
                    </div>
                    <div style={{ marginTop: "12px", display: "flex", gap: "8px", flexWrap: "wrap" }}>
                        {genres.map((g) => (
                            <Tag
                                key={g.Id ?? g.id}
                                style={{
                                    backgroundColor: "#1f1f1f",
                                    color: "#ccc",
                                    border: "1px solid #333",
                                    borderRadius: "4px",
                                }}
                            >
                                {g.Name ?? g.name}
                            </Tag>
                        ))}
                    </div>
                </div>
            </div>

            {/* Player */}
            <div style={{ marginBottom: "40px" }}>
                <Title
                    level={3}
                    style={{
                        color: "#fff",
                        marginBottom: "16px",
                        borderLeft: "4px solid #E50914",
                        paddingLeft: "12px",
                    }}
                >
                    Смотреть
                </Title>
                <VideoPlayer movieLinks={movieLinks} videoSources={videoSources} poster={poster} />
            </div>

            {/* Description */}
            <Row gutter={[32, 32]}>
                <Col xs={24} md={16}>
                    <Title level={4} style={{ color: "#fff" }}>
                        Описание
                    </Title>
                    <Paragraph style={{ color: "#ccc", fontSize: "16px", lineHeight: 1.7 }}>
                        {description || "Описание пока отсутствует."}
                    </Paragraph>
                </Col>
                <Col xs={24} md={8}>
                    <div
                        style={{
                            backgroundColor: "#1f1f1f",
                            borderRadius: "12px",
                            padding: "24px",
                        }}
                    >
                        <Title level={5} style={{ color: "#fff", marginTop: 0 }}>
                            Информация
                        </Title>
                        <Divider style={{ borderColor: "#333", margin: "12px 0" }} />
                        <Text style={{ color: "#888", display: "block" }}>Год</Text>
                        <Text style={{ color: "#fff", display: "block", marginBottom: 12 }}>{year}</Text>
                        <Text style={{ color: "#888", display: "block" }}>Режиссёр</Text>
                        <Text style={{ color: "#fff", display: "block", marginBottom: 12 }}>{director}</Text>
                        <Text style={{ color: "#888", display: "block" }}>Рейтинг КП</Text>
                        <Text style={{ color: "#E50914", fontWeight: 700 }}>
                            {Number(rating).toFixed(1)}
                        </Text>
                    </div>
                </Col>
            </Row>
        </div>
    );
};