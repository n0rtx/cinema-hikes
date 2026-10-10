import type{ MovieListItem } from "../CatalogDtos/MoveListItemDto";
export interface ViewHistoryDto {
  id: number;
  userId: number;
  movieId: number;
  movie: MovieListItem;
  progressSeconds: number;
  createdAt: string;
}
