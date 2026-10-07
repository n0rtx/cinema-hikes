import { useMemo, useState } from "react";
import { Select, Typography, Empty } from "antd";
import type { MovieLinkDto } from "../../../shared/types/DTO/CatalogDtos/MovieLinkDto";
import type { VideoSourceDto } from "../../../shared/types/DTO/CatalogDtos/VideoSourceDto";
import { VideoQuality } from "../../../shared/types/Enums/VideoQuality";

const { Text } = Typography;

interface VideoPlayerProps {
  movieLinks: MovieLinkDto[] | any[];
  videoSources: VideoSourceDto[] | any[];
  poster?: string;
}

const qualityLabel = (q: number | string) => {
  const map: Record<string, string> = {
    [VideoQuality.Low]: "360p",
    [VideoQuality.Medium]: "480p",
    [VideoQuality.High]: "720p",
    [VideoQuality.FullHd]: "1080p"
  };
  return map[String(q)] ?? String(q);
};

export const VideoPlayer = ({ movieLinks, videoSources, poster }: VideoPlayerProps) => {
  const directLinks = useMemo(() => {
    return (movieLinks || [])
      .map((l) => ({
        url: l.Url ?? l.url,
        quality: l.VideoQuality ?? l.videoQuality ?? l.Quality ?? l.quality,
        studio:
          l.TranslationStudioDto?.Name ??
          l.translationStudioDto?.name ??
          l.TranslationStudio?.Name ??
          "Озвучка",
      }))
      .filter((l) => !!l.url);
  }, [movieLinks]);

  const iframeSources = useMemo(() => {
    return (videoSources || [])
      .map((s) => ({
        url: s.PageUrl ?? s.pageUrl,
        name: s.ProviderName ?? s.providerName ?? "Источник",
        priority: s.Priority ?? s.priority ?? 0,
      }))
      .filter((s) => !!s.url)
      .sort((a, b) => a.priority - b.priority);
  }, [videoSources]);

  const [selectedLinkIdx, setSelectedLinkIdx] = useState(0);
  const [selectedSourceIdx, setSelectedSourceIdx] = useState(0);

  if (directLinks.length > 0) {
    const current = directLinks[selectedLinkIdx] ?? directLinks[0];
    return (
      <div>
        <div style={{ marginBottom: 12, display: "flex", gap: 12, flexWrap: "wrap" }}>
          <Select
            value={selectedLinkIdx}
            onChange={setSelectedLinkIdx}
            style={{ minWidth: 200 }}
            options={directLinks.map((l, i) => ({
              value: i,
              label: `${l.studio} · ${qualityLabel(l.quality)}`,
            }))}
          />
        </div>
        <div
          style={{
            position: "relative",
            width: "100%",
            paddingTop: "56.25%",
            backgroundColor: "#000",
            borderRadius: "12px",
            overflow: "hidden",
          }}
        >
          <video
            key={current.url}
            controls
            playsInline
            poster={poster}
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              width: "100%",
              height: "100%",
              backgroundColor: "#000",
            }}
            src={current.url}
          >
            Ваш браузер не поддерживает видео.
          </video>
        </div>
      </div>
    );
  }

  if (iframeSources.length > 0) {
    const current = iframeSources[selectedSourceIdx] ?? iframeSources[0];
    return (
      <div>
        <div style={{ marginBottom: 12 }}>
          <Select
            value={selectedSourceIdx}
            onChange={setSelectedSourceIdx}
            style={{ minWidth: 200 }}
            options={iframeSources.map((s, i) => ({
              value: i,
              label: s.name,
            }))}
          />
        </div>
        <div
          style={{
            position: "relative",
            width: "100%",
            paddingTop: "56.25%",
            backgroundColor: "#000",
            borderRadius: "12px",
            overflow: "hidden",
          }}
        >
          <iframe
            key={current.url}
            src={current.url}
            title={current.name}
            allowFullScreen
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              width: "100%",
              height: "100%",
              border: "none",
            }}
          />
        </div>
      </div>
    );
  }

  return (
    <div
      style={{
        backgroundColor: "#1f1f1f",
        borderRadius: "12px",
        padding: "48px",
        textAlign: "center",
      }}
    >
      <Empty description={<Text style={{ color: "#888" }}>Источники видео пока недоступны</Text>} />
    </div>
  );
};