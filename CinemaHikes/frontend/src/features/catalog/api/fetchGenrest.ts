import { axiosClient } from "../../../shared/api/axiosClient";
import type { GenreDto } from "../../../shared/types/DTO/CatalogDtos/GenreDto";
export const fetchGenres = async (): Promise<GenreDto[]> => {
  const { data } = await axiosClient.get<GenreDto[]>("/api/genres");
  return data;
};