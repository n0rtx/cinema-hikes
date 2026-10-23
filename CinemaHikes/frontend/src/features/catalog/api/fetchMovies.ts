import { axiosClient } from "../../../shared/api/axiosClient";
import type { MovieFilters } from "../../../shared/types/DTO/CatalogDtos/MovieFiltersDto";
import type { MoviesPage } from "../../../shared/types/DTO/CatalogDtos/MoviesPageDto";
import type { MovieListItem } from "../../../shared/types/DTO/CatalogDtos/MoveListItemDto";

type Raw = { items: MovieListItem[]; total: number };

export const fetchMovies = async (f: MovieFilters): Promise<MoviesPage> => {
  const search = f.search?.trim();

  if (search) {
    const { data } = await axiosClient.get<Raw>("/api/movies/search", {
      params: { query: search, page: f.page, pageSize: f.pageSize },
    });
    return { items: data.items ?? [], total: data.total ?? 0 };
  }

  const { data } = await axiosClient.get<Raw>("/api/movies", {
    params: {
      genreIds: f.genreIds?.length ? f.genreIds : undefined,
      year: f.year,
      minRating: f.minRating,
      sort: f.sort,
      page: f.page,
      pageSize: f.pageSize,
    },
    paramsSerializer: { indexes: null },
  });
  return { items: data.items ?? [], total: data.total ?? 0 };
};