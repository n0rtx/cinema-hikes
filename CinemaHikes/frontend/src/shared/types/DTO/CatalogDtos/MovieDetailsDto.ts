import type { GenreDto } from "./GenreDto";
import type { MovieLinkDto } from "./MovieLinkDto";
import type { ReviewDto } from "./ReviewDto";
import type { VideoSourceDto } from "./VideoSourceDto";

export interface MovieDetailsDto {
  id: number;
  ruTitle: string;
  uaTitle: string;
  ruInEngTitle: string;
  description: string;
  director: string;
  releaseYear: number;
  posterUrl: string;
  kpRating: number;
  createdAt: string;
  genres: GenreDto[];
  videoSources: VideoSourceDto[];
  movieLinks: MovieLinkDto[];
  reviews: ReviewDto[];
}