
import { useEffect, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useDebounce } from "../../shared/hooks/useDebounce";
import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { fetchGenres } from "../../features/catalog/api/fetchGenres";
import { fetchMovies } from "../../features/catalog/api/fetchMovies";
import "../../features/catalog/components/CatalogPage.css";
import {
  Input,
  Select,
  Pagination,
  Skeleton,
  Button,
  Typography,
} from "antd";
import {
  SearchOutlined,
  PlayCircleOutlined,
  VideoCameraOutlined,
  ReloadOutlined,
} from "@ant-design/icons";
import { getMovieTitle } from "../../shared/utils/getMovieTitle";
import i18n from "../../shared/i18n";
const { Text, Title } = Typography;
const Page_Size = 24;
export const CatalogPage = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [params, setParams] = useSearchParams();
  const genreId = params.get("genre") ? Number(params.get("genre")) : undefined;
  const year = params.get("year") ? Number(params.get("year")) : undefined;
  const minRating = params.get("rating")
    ? Number(params.get("rating"))
    : undefined;
  const sort = params.get("sort") ?? "rating";
  const page = Number(params.get("page") ?? 1);
  const urlSearch = params.get("q") ?? "";
  const [searchInput, setSearchInput] = useState(urlSearch);
  const debouncedSearch = useDebounce(searchInput, 400);
  const update = (patch: Record<string, string | number | undefined>) => {
    const next = new URLSearchParams(params);
    Object.entries(patch).forEach(([k, v]) => {
      if (v === undefined || v === "") next.delete(k);
      else next.set(k, String(v));
    });
    if (!("page" in patch)) next.delete("page");
    setParams(next, { replace: true });
  };
  useEffect(() => {
    if (debouncedSearch !== urlSearch) update({ q: debouncedSearch });
  }, [debouncedSearch]);
  const { data: genres = [] } = useQuery({
    queryKey: ["genres"],
    queryFn: fetchGenres,
    staleTime: 10 * 60 * 1000,
  });

  const { data, isLoading, isFetching, isError, refetch } = useQuery({
    queryKey: ["movies", urlSearch, genreId, year, minRating, sort, page],
    queryFn: () =>
      fetchMovies({
        search: urlSearch,
        genreId,
        year,
        minRating,
        sort,
        page,
        pageSize: Page_Size,
      }),
    placeholderData: keepPreviousData,
  });

  const years = useMemo(() => {
    const current = new Date().getFullYear();
    return Array.from({ length: current - 1949 }, (_, i) => current - i);
  }, []);
  const hasFilters = !!(urlSearch || genreId || year || minRating);
  const resetAll = () => {
    setSearchInput("");
    setParams({}, { replace: true });
  };
  const selectStyle = { minWidth: 140 };
  return (
    <div
      style={{ maxWidth: 1200, margin: "0 auto", padding: "32px 16px 56px" }}
    >
      <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
        <div
          style={{
            width: 4,
            height: 32,
            borderRadius: 2,
            backgroundColor: "#E50914",
          }}
        />
        <Title level={2} style={{ color: "#fff", margin: 0 }}>
          {t("catalog.title")}
        </Title>
        {data && (
          <Text style={{ color: "#888", marginLeft: 4 }}>
            {t("catalog.found", { count: data.total })}
          </Text>
        )}
      </div>

     
      <div className="cat-filters">
        <div style={{ display: "flex", flexWrap: "wrap", gap: 12 }}>
          <Input
            allowClear
            size="large"
            prefix={<SearchOutlined />}
            placeholder={t("catalog.searchPlaceholder")}
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            style={{ flex: "1 1 260px", maxWidth: 420 }}
          />
          <Select
            size="large"
            allowClear
            showSearch
            placeholder={t("catalog.year")}
            value={year}
            onChange={(v) => update({ year: v })}
            options={years.map((y) => ({ value: y, label: y }))}
            style={selectStyle}
          />
          <Select
            size="large"
            allowClear
            placeholder={t("catalog.minRating")}
            value={minRating}
            onChange={(v) => update({ rating: v })}
            options={[9, 8, 7, 6, 5].map((r) => ({
              value: r,
              label: `★ ${r}+`,
            }))}
            style={selectStyle}
          />
          <Select
            size="large"
            value={sort}
            onChange={(v) => update({ sort: v })}
            options={[
              { value: "rating", label: t("catalog.sortRating") },
              { value: "year", label: t("catalog.sortYear") },
              { value: "title", label: t("catalog.sortTitle") },
            ]}
            style={{ minWidth: 170 }}
          />
          {hasFilters && (
            <Button
              size="large"
              icon={<ReloadOutlined />}
              onClick={resetAll}
              style={{ borderColor: "#E50914", color: "#E50914" }}
            >
              {t("catalog.reset")}
            </Button>
          )}
        </div>

        <div className="cat-chips">
          <span
            className={`cat-chip ${!genreId ? "active" : ""}`}
            onClick={() => update({ genre: undefined })}
          >
            {t("catalog.allGenres")}
          </span>
          {genres.map((g) => (
            <span
              key={g.id}
              className={`cat-chip ${genreId === g.id ? "active" : ""}`}
              onClick={() => update({ genre: g.id })}
            >
              {g.name}
            </span>
          ))}
        </div>
      </div>

      
      {isLoading ? (
        <div className="cat-grid">
          {Array.from({ length: 12 }).map((_, i) => (
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
      ) : isError ? (
        <div className="cat-empty">
          <Text style={{ color: "#999" }}>{t("catalog.loadFailed")}</Text>
          <Button
            type="primary"
            onClick={() => refetch()}
            style={{ backgroundColor: "#E50914", borderColor: "#E50914" }}
          >
            {t("catalog.retry")}
          </Button>
        </div>
      ) : data && data.items.length === 0 ? (
        <div className="cat-empty">
          <VideoCameraOutlined style={{ fontSize: 48, color: "#E50914" }} />
          <Title level={4} style={{ color: "#fff", margin: 0 }}>
            {t("catalog.emptyTitle")}
          </Title>
          <Text style={{ color: "#999" }}>{t("catalog.emptyText")}</Text>
          {hasFilters && (
            <Button
              type="primary"
              onClick={resetAll}
              style={{ backgroundColor: "#E50914", borderColor: "#E50914" }}
            >
              {t("catalog.reset")}
            </Button>
          )}
        </div>
      ) : (
        <div
          className="cat-grid"
          style={{ opacity: isFetching ? 0.6 : 1, transition: "opacity .2s" }}
        >
          {data?.items.map((m) => (
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


      {data && data.total > Page_Size && (
        <div
          style={{ display: "flex", justifyContent: "center", marginTop: 36 }}
        >
          <Pagination
            current={page}
            pageSize={Page_Size}
            total={data.total}
            showSizeChanger={false}
            onChange={(p) => {
              update({ page: p });
              window.scrollTo({ top: 0, behavior: "smooth" });
            }}
          />
        </div>
      )}
    </div>
  );
};
