import { axiosClient } from "../../../shared/api/axiosClient";
import type { MovieFilters } from "../../../shared/types/DTO/CatalogDtos/MovieFiltersDto";
import type { MoviesPage } from "../../../shared/types/DTO/CatalogDtos/MoviesPageDto";
import type { MovieListItem } from "../../../shared/types/DTO/CatalogDtos/MoveListItemDto";

export const fetchMovies = async (f: MovieFilters): Promise<MoviesPage> => {
  const hasSearch = Boolean(f.search?.trim());

  if (hasSearch) {
    const { data } = await axiosClient.get<MovieListItem[]>("/api/movies/search", {
      params: {
        query: f.search!.trim(),
        page: f.page,
        pageSize: f.pageSize,
      },
    });

    const items = Array.isArray(data) ? data : [];
    return { items, total: items.length };
  }

  const { data } = await axiosClient.get("/api/movies", {
    params: {
      genreId: f.genreId,
      year: f.year,
      minRating: f.minRating,
      sort: f.sort,
      page: f.page,
      pageSize: f.pageSize,
    },
  });

  if (Array.isArray(data)) return { items: data, total: data.length };
  return {
    items: data.items ?? [],
    total: data.total ?? data.totalCount ?? 0,
  };
};