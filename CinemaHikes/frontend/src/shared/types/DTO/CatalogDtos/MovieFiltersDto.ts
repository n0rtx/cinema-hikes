export interface MovieFilters {
  search?: string;
  genreId?: number;
  year?: number;
  minRating?: number;
  sort?: string;
  page: number;
  pageSize: number;
}