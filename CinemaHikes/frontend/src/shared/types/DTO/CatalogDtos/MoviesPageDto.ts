import type{ MovieListItem } from "./MoveListItemDto";
export interface MoviesPage {
  items: MovieListItem[];
  total: number;
}