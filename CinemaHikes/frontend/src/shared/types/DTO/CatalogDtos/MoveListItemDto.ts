import type { GenreDto } from "./GenreDto";

export interface MovieListItem {
  id: number;
  ruTitle: string;
  uaTitle?: string;
  ruInEngTitle: string;
  director?:string;
  description?:string
  releaseYear: number;
  posterUrl: string;
  kpRating: number;
  genres: GenreDto[];
}