import { useMemo, useState, useEffect, useRef, useCallback } from "react";
import { Typography, Empty } from "antd";
import type { MovieLinkDto } from "../../../shared/types/DTO/CatalogDtos/MovieLinkDto";
import type { VideoSourceDto } from "../../../shared/types/DTO/CatalogDtos/VideoSourceDto";
import { VideoQuality } from "../../../shared/types/Enums/VideoQuality";

const { Text } = Typography;

interface VideoPlayerProps {
  movieLinks: MovieLinkDto[];
  videoSources: VideoSourceDto[];
  poster?: string;
}

const QUALITY_ORDER = [
  VideoQuality.FullHd,
  VideoQuality.High,
  VideoQuality.Medium,
  VideoQuality.Low,
];

const qualityLabel = (q: number | string) => {
  const map: Record<string, string> = {
    [VideoQuality.Low]: "360p",
    [VideoQuality.Medium]: "480p",
    [VideoQuality.High]: "720p",
    [VideoQuality.FullHd]: "1080p",
  };
  return map[String(q)] ?? `${q}p`;
};

const isMp4Url = (url: string | undefined | null): boolean => {
  if (!url) return false;
  const clean = url.split("?")[0].split("#")[0].toLowerCase();
  if (clean.includes(".m3u8") || clean.includes("/hls/")) return false;
  if (clean.endsWith(".mp4")) return true;
  return !clean.includes("m3u8");
};

const formatTime = (sec: number) => {
  if (!Number.isFinite(sec) || sec < 0) return "0:00";
  const m = Math.floor(sec / 60);
  const s = Math.floor(sec % 60);
  return `${m}:${s.toString().padStart(2, "0")}`;
};

const chipBase: React.CSSProperties = {
  display: "inline-flex",
  alignItems: "center",
  padding: "6px 14px",
  borderRadius: "4px",
  fontSize: "13px",
  fontWeight: 500,
  cursor: "pointer",
  border: "1px solid #333",
  backgroundColor: "#1a1a1a",
  color: "#ccc",
  transition: "all 0.15s ease",
  userSelect: "none",
  whiteSpace: "nowrap",
};

const chipActive: React.CSSProperties = {
  ...chipBase,
  backgroundColor: "#E50914",
  borderColor: "#E50914",
  color: "#fff",
};

const sectionLabel: React.CSSProperties = {
  color: "#888",
  fontSize: "12px",
  textTransform: "uppercase",
  letterSpacing: "0.04em",
  marginBottom: 8,
  display: "block",
};

const iconButton: React.CSSProperties = {
  background: "none",
  border: "none",
  cursor: "pointer",
  padding: 4,
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
};

export const VideoPlayer = ({ movieLinks, videoSources, poster }: VideoPlayerProps) => {
  const videoRef = useRef<HTMLVideoElement>(null);
  const barRef = useRef<HTMLDivElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  const [playing, setPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [buffered, setBuffered] = useState(0);
  const [volume, setVolume] = useState(1);
  const [muted, setMuted] = useState(false);
  const [showControls, setShowControls] = useState(true);
  const [isFullscreen, setIsFullscreen] = useState(false);
  const hideTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  const mp4Links = useMemo(
    () => (movieLinks ?? []).filter((l) => isMp4Url(l.url)),
    [movieLinks]
  );

  const studios = useMemo(() => {
    const map = new Map<string, { id: number; name: string }>();
    for (const l of mp4Links) {
      const id = l.translationStudio?.id ?? 0;
      const name = l.translationStudio?.name ?? "Озвучка";
      const key = `${id}:${name}`;
      if (!map.has(key)) map.set(key, { id, name });
    }
    return Array.from(map.values());
  }, [mp4Links]);

  const providers = useMemo(() => {
    return (videoSources ?? [])
      .map((s) => ({
        id: s.id,
        name: s.providerName || "Источник",
        url: s.pageUrl,
        priority: s.priority ?? 0,
      }))
      .filter((s) => !!s.url)
      .sort((a, b) => a.priority - b.priority);
  }, [videoSources]);

  const hasDirect = mp4Links.length > 0;
  const hasIframe = providers.length > 0;

  const [mode, setMode] = useState<"direct" | "iframe">(
    hasDirect ? "direct" : "iframe"
  );
  const [studioKey, setStudioKey] = useState("");
  const [quality, setQuality] = useState<number | null>(null);
  const [providerIdx, setProviderIdx] = useState(0);

  useEffect(() => {
    if (studios.length > 0 && !studioKey) {
      setStudioKey(`${studios[0].id}:${studios[0].name}`);
    }
  }, [studios, studioKey]);

  const studioLinks = useMemo(() => {
    if (!studioKey) return mp4Links;
    const [idStr, ...nameParts] = studioKey.split(":");
    const id = Number(idStr);
    const name = nameParts.join(":");
    return mp4Links.filter((l) => {
      const sid = l.translationStudio?.id ?? 0;
      const sname = l.translationStudio?.name ?? "Озвучка";
      return sid === id && sname === name;
    });
  }, [mp4Links, studioKey]);

  const qualities = useMemo(() => {
    const set = new Set<number>();
    for (const l of studioLinks) {
      if (l.url && l.videoQuality != null) set.add(Number(l.videoQuality));
    }
    return QUALITY_ORDER.filter((q) => set.has(q));
  }, [studioLinks]);

  useEffect(() => {
    if (qualities.length === 0) {
      setQuality(null);
      return;
    }
    if (quality == null || !qualities.includes(quality as VideoQuality)) {
      setQuality(qualities[0]);
    }
  }, [qualities, quality]);

  const currentDirectUrl = useMemo(() => {
    if (quality == null) return studioLinks.find((l) => l.url)?.url ?? "";
    return (
      studioLinks.find((l) => Number(l.videoQuality) === quality && l.url)?.url ??
      studioLinks.find((l) => l.url)?.url ??
      ""
    );
  }, [studioLinks, quality]);

  const currentIframe = providers[providerIdx] ?? providers[0];

  // reset playback state when source changes
  useEffect(() => {
    setPlaying(false);
    setCurrentTime(0);
    setDuration(0);
    setBuffered(0);
  }, [currentDirectUrl]);

  const scheduleHide = useCallback(() => {
    if (hideTimer.current) clearTimeout(hideTimer.current);
    setShowControls(true);
    hideTimer.current = setTimeout(() => {
      if (videoRef.current && !videoRef.current.paused) {
        setShowControls(false);
      }
    }, 2500);
  }, []);

  const togglePlay = () => {
    const v = videoRef.current;
    if (!v) return;
    if (v.paused) {
      void v.play();
      setPlaying(true);
    } else {
      v.pause();
      setPlaying(false);
    }
    scheduleHide();
  };

  const onTimeUpdate = () => {
    const v = videoRef.current;
    if (!v) return;
    setCurrentTime(v.currentTime);
    if (v.buffered.length > 0) {
      setBuffered(v.buffered.end(v.buffered.length - 1));
    }
  };

  const onLoadedMetadata = () => {
    const v = videoRef.current;
    if (!v) return;
    setDuration(v.duration);
  };

  const seekToClientX = (clientX: number) => {
    const bar = barRef.current;
    const v = videoRef.current;
    if (!bar || !v || !duration) return;
    const rect = bar.getBoundingClientRect();
    const ratio = Math.min(1, Math.max(0, (clientX - rect.left) / rect.width));
    v.currentTime = ratio * duration;
    setCurrentTime(v.currentTime);
  };

  const onBarClick = (e: React.MouseEvent) => {
    seekToClientX(e.clientX);
  };

  const onBarDrag = (e: React.MouseEvent) => {
    if (e.buttons !== 1) return;
    seekToClientX(e.clientX);
  };

  const toggleMute = () => {
    const v = videoRef.current;
    if (!v) return;
    v.muted = !v.muted;
    setMuted(v.muted);
  };

  const onVolumeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const v = videoRef.current;
    const val = Number(e.target.value);
    if (!v) return;
    v.volume = val;
    setVolume(val);
    if (val > 0 && v.muted) {
      v.muted = false;
      setMuted(false);
    }
  };

  useEffect(() => {
    const onChange = () =>
      setIsFullscreen(document.fullscreenElement === containerRef.current);
    document.addEventListener("fullscreenchange", onChange);
    return () => document.removeEventListener("fullscreenchange", onChange);
  }, []);

  const toggleFullscreen = () => {
    const el = containerRef.current;
    if (!el) return;
    if (document.fullscreenElement) {
      void document.exitFullscreen();
    } else {
      void el.requestFullscreen?.();
    }
    scheduleHide();
  };

  const progressPct = duration > 0 ? (currentTime / duration) * 100 : 0;
  const bufferedPct = duration > 0 ? (buffered / duration) * 100 : 0;

  if (!hasDirect && !hasIframe) {
    return (
      <div
        style={{
          backgroundColor: "#1a1a1a",
          borderRadius: "8px",
          padding: "64px 24px",
          textAlign: "center",
          border: "1px solid #2a2a2a",
        }}
      >
        <Empty
          description={
            <Text style={{ color: "#888" }}>
              Источники видео пока недоступны (нужны MP4-ссылки)
            </Text>
          }
        />
      </div>
    );
  }

  return (
    <div>
      {/* TOP CONTROLS */}
      <div
        style={{
          backgroundColor: "#1a1a1a",
          border: "1px solid #2a2a2a",
          borderBottom: "none",
          borderRadius: "8px 8px 0 0",
          padding: "14px 16px",
          display: "flex",
          flexDirection: "column",
          gap: 14,
        }}
      >
        {hasDirect && hasIframe && (
          <div>
            <Text style={sectionLabel}>Тип</Text>
            <div style={{ display: "flex", gap: 8, flexWrap: "wrap" }}>
              <button
                type="button"
                onClick={() => setMode("direct")}
                style={mode === "direct" ? chipActive : chipBase}
              >
                MP4
              </button>
              <button
                type="button"
                onClick={() => setMode("iframe")}
                style={mode === "iframe" ? chipActive : chipBase}
              >
                Плеер источника
              </button>
            </div>
          </div>
        )}

        {mode === "direct" && studios.length > 0 && (
          <div>
            <Text style={sectionLabel}>Озвучка / студия</Text>
            <div style={{ display: "flex", gap: 8, flexWrap: "wrap" }}>
              {studios.map((s) => {
                const key = `${s.id}:${s.name}`;
                return (
                  <button
                    key={key}
                    type="button"
                    onClick={() => setStudioKey(key)}
                    style={studioKey === key ? chipActive : chipBase}
                  >
                    {s.name}
                  </button>
                );
              })}
            </div>
          </div>
        )}

        {(mode === "iframe" || (!hasDirect && hasIframe)) && providers.length > 0 && (
          <div>
            <Text style={sectionLabel}>Источник</Text>
            <div style={{ display: "flex", gap: 8, flexWrap: "wrap" }}>
              {providers.map((p, i) => (
                <button
                  key={p.id}
                  type="button"
                  onClick={() => {
                    setProviderIdx(i);
                    setMode("iframe");
                  }}
                  style={providerIdx === i ? chipActive : chipBase}
                >
                  {p.name}
                </button>
              ))}
            </div>
          </div>
        )}
      </div>

      {/* VIDEO BLOCK */}
      <div
        ref={containerRef}
        style={{
          position: "relative",
          width: "100%",
          height: isFullscreen ? "100%" : undefined,
          paddingTop: isFullscreen ? 0 : "56.25%",
          backgroundColor: "#000",
          borderRadius: isFullscreen ? 0 : "0 0 8px 8px",
          overflow: "hidden",
          border: isFullscreen ? "none" : "1px solid #2a2a2a",
          borderTop: "none",
        }}
        onMouseMove={mode === "direct" ? scheduleHide : undefined}
        onMouseLeave={() => {
          if (playing) setShowControls(false);
        }}
      >
        {/* Quality pills */}
        {mode === "direct" && qualities.length > 0 && (
          <div
            style={{
              position: "absolute",
              top: 12,
              right: 12,
              zIndex: 6,
              display: "flex",
              gap: 6,
              opacity: showControls ? 1 : 0,
              transition: "opacity 0.2s",
            }}
          >
            {qualities.map((q) => (
              <button
                key={q}
                type="button"
                onClick={() => setQuality(q)}
                style={{
                  padding: "4px 10px",
                  borderRadius: "3px",
                  fontSize: "12px",
                  fontWeight: 700,
                  cursor: "pointer",
                  border:
                    quality === q
                      ? "1px solid #E50914"
                      : "1px solid rgba(255,255,255,0.25)",
                  backgroundColor:
                    quality === q
                      ? "rgba(229, 9, 20, 0.9)"
                      : "rgba(0, 0, 0, 0.65)",
                  color: "#fff",
                  userSelect: "none",
                }}
              >
                {qualityLabel(q)}
              </button>
            ))}
          </div>
        )}

        {mode === "direct" && currentDirectUrl ? (
          <>
            <video
              ref={videoRef}
              key={currentDirectUrl}
              playsInline
              preload="metadata"
              poster={poster}
              onClick={togglePlay}
              onTimeUpdate={onTimeUpdate}
              onLoadedMetadata={onLoadedMetadata}
              onPlay={() => setPlaying(true)}
              onPause={() => setPlaying(false)}
              onEnded={() => setPlaying(false)}
              style={{
                position: "absolute",
                top: 0,
                left: 0,
                width: "100%",
                height: "100%",
                backgroundColor: "#000",
                cursor: "pointer",
              }}
            >
              <source src={currentDirectUrl} type="video/mp4" />
            </video>

            {/* Center play button (when paused) */}
            {!playing && (
              <button
                type="button"
                onClick={togglePlay}
                aria-label="Play"
                style={{
                  position: "absolute",
                  top: "50%",
                  left: "50%",
                  transform: "translate(-50%, -50%)",
                  zIndex: 5,
                  width: 72,
                  height: 72,
                  borderRadius: "50%",
                  border: "none",
                  backgroundColor: "#E50914",
                  cursor: "pointer",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  boxShadow: "0 4px 20px rgba(229, 9, 20, 0.45)",
                  padding: 0,
                }}
              >
                {/* white triangle */}
                <span
                  style={{
                    width: 0,
                    height: 0,
                    marginLeft: 4,
                    borderTop: "12px solid transparent",
                    borderBottom: "12px solid transparent",
                    borderLeft: "20px solid #fff",
                  }}
                />
              </button>
            )}

            {/* Bottom controls bar */}
            <div
              style={{
                position: "absolute",
                left: 0,
                right: 0,
                bottom: 0,
                zIndex: 5,
                padding: "8px 12px 10px",
                background:
                  "linear-gradient(transparent, rgba(0,0,0,0.85))",
                opacity: showControls || !playing ? 1 : 0,
                transition: "opacity 0.2s",
                pointerEvents: showControls || !playing ? "auto" : "none",
              }}
            >
              {/* Progress bar — RED */}
              <div
                ref={barRef}
                onClick={onBarClick}
                onMouseMove={onBarDrag}
                style={{
                  position: "relative",
                  height: 5,
                  borderRadius: 3,
                  backgroundColor: "rgba(255,255,255,0.2)",
                  cursor: "pointer",
                  marginBottom: 10,
                }}
              >
                {/* buffered */}
                <div
                  style={{
                    position: "absolute",
                    left: 0,
                    top: 0,
                    bottom: 0,
                    width: `${bufferedPct}%`,
                    backgroundColor: "rgba(255,255,255,0.3)",
                    borderRadius: 3,
                  }}
                />
                {/* played — RED */}
                <div
                  style={{
                    position: "absolute",
                    left: 0,
                    top: 0,
                    bottom: 0,
                    width: `${progressPct}%`,
                    backgroundColor: "#E50914",
                    borderRadius: 3,
                  }}
                />
                {/* thumb */}
                <div
                  style={{
                    position: "absolute",
                    top: "50%",
                    left: `${progressPct}%`,
                    width: 12,
                    height: 12,
                    borderRadius: "50%",
                    backgroundColor: "#E50914",
                    border: "2px solid #fff",
                    transform: "translate(-50%, -50%)",
                    boxShadow: "0 0 4px rgba(0,0,0,0.5)",
                  }}
                />
              </div>

              <div
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: 12,
                }}
              >
                {/* Small round play/pause */}
                <button
                  type="button"
                  onClick={togglePlay}
                  aria-label={playing ? "Pause" : "Play"}
                  style={{
                    width: 36,
                    height: 36,
                    borderRadius: "50%",
                    border: "none",
                    backgroundColor: "#E50914",
                    cursor: "pointer",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    flexShrink: 0,
                    padding: 0,
                  }}
                >
                  {playing ? (
                    /* pause icon */
                    <span style={{ display: "flex", gap: 3 }}>
                      <span
                        style={{
                          width: 3,
                          height: 12,
                          backgroundColor: "#fff",
                          borderRadius: 1,
                        }}
                      />
                      <span
                        style={{
                          width: 3,
                          height: 12,
                          backgroundColor: "#fff",
                          borderRadius: 1,
                        }}
                      />
                    </span>
                  ) : (
                    <span
                      style={{
                        width: 0,
                        height: 0,
                        marginLeft: 2,
                        borderTop: "6px solid transparent",
                        borderBottom: "6px solid transparent",
                        borderLeft: "10px solid #fff",
                      }}
                    />
                  )}
                </button>

                <Text style={{ color: "#ddd", fontSize: 12, minWidth: 90 }}>
                  {formatTime(currentTime)} / {formatTime(duration)}
                </Text>

                <div style={{ flex: 1 }} />

                {/* Volume */}
                <button
                  type="button"
                  onClick={toggleMute}
                  aria-label={muted || volume === 0 ? "Unmute" : "Mute"}
                  style={iconButton}
                >
                  <svg
                    width="22"
                    height="22"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="#fff"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    aria-hidden="true"
                  >
                    <polygon points="11 5 6 9 2 9 2 15 6 15 11 19 11 5" fill="#fff" />
                    {muted || volume === 0 ? (
                      <>
                        <line x1="22" y1="9" x2="16" y2="15" />
                        <line x1="16" y1="9" x2="22" y2="15" />
                      </>
                    ) : (
                      <>
                        {volume > 0.5 && <path d="M19.07 4.93a10 10 0 0 1 0 14.14" />}
                        <path d="M15.54 8.46a5 5 0 0 1 0 7.07" />
                      </>
                    )}
                  </svg>
                </button>
                <input
                  type="range"
                  min={0}
                  max={1}
                  step={0.05}
                  value={muted ? 0 : volume}
                  onChange={onVolumeChange}
                  style={{
                    width: 80,
                    accentColor: "#E50914",
                    cursor: "pointer",
                  }}
                />

                {/* Fullscreen */}
                <button
                  type="button"
                  onClick={toggleFullscreen}
                  aria-label={isFullscreen ? "Exit fullscreen" : "Fullscreen"}
                  style={iconButton}
                >
                  <svg
                    width="22"
                    height="22"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="#fff"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    aria-hidden="true"
                  >
                    {isFullscreen ? (
                      <path d="M8 3v3a2 2 0 0 1-2 2H3m18 0h-3a2 2 0 0 1-2-2V3m0 18v-3a2 2 0 0 1 2-2h3M3 16h3a2 2 0 0 1 2 2v3" />
                    ) : (
                      <path d="M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3" />
                    )}
                  </svg>
                </button>
              </div>
            </div>
          </>
        ) : mode === "iframe" && currentIframe ? (
          <iframe
            key={currentIframe.url}
            src={currentIframe.url}
            title={currentIframe.name}
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
        ) : (
          <div
            style={{
              position: "absolute",
              inset: 0,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              backgroundColor: "#0a0a0a",
            }}
          >
            <Text style={{ color: "#666" }}>
              {mode === "direct"
                ? "Нет доступных MP4-ссылок"
                : "Выберите источник"}
            </Text>
          </div>
        )}
      </div>
    </div>
  );
};