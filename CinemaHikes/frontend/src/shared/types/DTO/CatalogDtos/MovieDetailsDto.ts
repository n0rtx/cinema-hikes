import type { GenreDto } from "./GenreDto";
import type { MovieLinkDto } from "./MovieLinkDto";
import type { ReviewDto } from "./ReviewDto";
import type { VideoSourceDto } from "./VideoSourceDto";

export interface MovieDetailsDto {
  Id: string;
  RuTitle: string;
  UaTitle: string;
  RuInEngTitle: string;
  Description: string;
  Director: string;
  ReleaseYear: number;
  PosterUrl: string;
  KpRating: number;
  Genres:GenreDto[];
  VideoSources:VideoSourceDto[];
  MovieLinks:MovieLinkDto[];
  Reviews:ReviewDto[];
}
