import type{ MovieListItem } from "../CatalogDtos/MoveListItemDto";
export interface FavoriteDto {
  id: number;
  userId: number;
  movieId: number;
  movie: MovieListItem;
  createdAt: string;
}