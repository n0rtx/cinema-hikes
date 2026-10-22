import { useQuery } from "@tanstack/react-query";
import { axiosClient } from "../../../shared/api/axiosClient";
import type { MovieListItem } from "../../../shared/types/DTO/CatalogDtos/MoveListItemDto";

export interface MoviesQueryParams {
  genreId?: number;
  year?: number;
  minRating?: number;
  onlyWithPoster?: boolean;
  page?: number;
  pageSize?: number;
}

async function fetchMovies(params: MoviesQueryParams = {}): Promise<MovieListItem[]> {
  const { data } = await axiosClient.get<{ items: MovieListItem[]; total: number }>(
    "/api/movies",
    {
      params: {
        genreId: params.genreId,
        year: params.year,
        minRating: params.minRating,
        onlyWithPoster: params.onlyWithPoster,
        page: params.page ?? 1,
        pageSize: params.pageSize ?? 20,
      },
    }
  );
  return data.items ?? [];
}

export function useMoviesQuery(params: MoviesQueryParams = {}) {
  return useQuery({
    queryKey: ["movies", params],
    queryFn: () => fetchMovies(params),
  });
}