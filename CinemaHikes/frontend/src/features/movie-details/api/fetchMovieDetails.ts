import { axiosClient } from "../../../shared/api/axiosClient";
import type { MovieDetailsDto } from "../../../shared/types/DTO/CatalogDtos/MovieDetailsDto";

export async function fetchMovieDetails(id: number): Promise<MovieDetailsDto> {
  const { data } = await axiosClient.get<MovieDetailsDto>(`/api/movies/${id}`);
  return data;
}