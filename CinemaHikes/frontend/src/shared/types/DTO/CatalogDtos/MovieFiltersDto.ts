export interface MovieFilters {
  search?: string;
  genreIds?: number[];
  year?: number;
  minRating?: number;
  sort?: string;
  page: number;
  pageSize: number;
}