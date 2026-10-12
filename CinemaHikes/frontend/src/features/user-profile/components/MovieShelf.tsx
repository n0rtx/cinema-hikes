import { useRef, type ReactNode } from "react";
import { Button, Skeleton, Typography } from "antd";
import {
  LeftOutlined,
  RightOutlined,
  HeartFilled,
  VideoCameraOutlined,
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { formatDate } from "../../../shared/utils/dateUtils";
import "./MovieShelf.css";

const { Title, Text } = Typography;

export interface ShelfItem {
  id: number;
  title: string;
  posterUrl?: string;
  year?: number;
  rating?: number;
  date?: string;
}

interface Props {
  title: string;
  icon: ReactNode;
  items: ShelfItem[];
  loading: boolean;
  emptyText: string;
  dateLabelKey?: string; 
  showHeart?: boolean;
}

export const MovieShelf = ({
  title,
  icon,
  items,
  loading,
  emptyText,
  dateLabelKey,
  showHeart,
}: Props) => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const rowRef = useRef<HTMLDivElement>(null);

  const scroll = (dir: 1 | -1) =>
    rowRef.current?.scrollBy({ left: dir * 480, behavior: "smooth" });

  return (
    <section style={{ marginTop: "40px" }}>
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          marginBottom: "8px",
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: "12px" }}>
          <div
            style={{
              width: 4,
              height: 26,
              borderRadius: 2,
              backgroundColor: "#E50914",
            }}
          />
          <span style={{ color: "#E50914", fontSize: 20 }}>{icon}</span>
          <Title level={4} style={{ color: "#fff", margin: 0 }}>
            {title}
          </Title>
          {!loading && items.length > 0 && (
            <span
              style={{
                padding: "0 10px",
                borderRadius: 999,
                backgroundColor: "#2a2a2a",
                color: "#aaa",
                fontSize: 12,
                lineHeight: "22px",
              }}
            >
              {items.length}
            </span>
          )}
        </div>

        {items.length > 3 && (
          <div style={{ display: "flex", gap: 8 }}>
            <Button
              shape="circle"
              size="small"
              icon={<LeftOutlined />}
              onClick={() => scroll(-1)}
            />
            <Button
              shape="circle"
              size="small"
              icon={<RightOutlined />}
              onClick={() => scroll(1)}
            />
          </div>
        )}
      </div>

      {loading ? (
        <div style={{ display: "flex", gap: 16, padding: "10px 4px 18px" }}>
          {Array.from({ length: 5 }).map((_, i) => (
            <Skeleton.Node
              key={i}
              active
              style={{ width: 150, height: 225, borderRadius: 12 }}
            >
              {" "}
            </Skeleton.Node>
          ))}
        </div>
      ) : items.length === 0 ? (
        <div className="shelf-empty">
          <VideoCameraOutlined style={{ fontSize: 32, color: "#E50914" }} />
          <Text style={{ color: "#999" }}>{emptyText}</Text>
          <Button
            type="primary"
            onClick={() => navigate("/catalog")}
            style={{ backgroundColor: "#E50914", borderColor: "#E50914" }}
          >
            {t("profile.goToCatalog")}
          </Button>
        </div>
      ) : (
        <div className="shelf-row" ref={rowRef}>
          {items.map((m) => (
            <div
              key={m.id}
              className="shelf-card"
              onClick={() => navigate(`/movies/${m.id}`)}
            >
              <div className="shelf-poster">
                {m.posterUrl ? (
                  <img src={m.posterUrl} alt={m.title} loading="lazy" />
                ) : (
                  <div className="shelf-fallback">
                    <VideoCameraOutlined />
                  </div>
                )}

                {m.rating ? (
                  <span className="shelf-badge">★ {m.rating.toFixed(1)}</span>
                ) : null}
                {showHeart && <HeartFilled className="shelf-heart" />}

                <div className="shelf-overlay">{t("profile.watchNow")}</div>
              </div>

              <div className="shelf-title">{m.title}</div>
              <div className="shelf-meta">
                {dateLabelKey && m.date
                  ? t(dateLabelKey, { date: formatDate(m.date) })
                  : m.year ?? ""}
              </div>
            </div>
          ))}
        </div>
      )}
    </section>
  );
};