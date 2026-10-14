import type { MovieListItem } from "../CatalogDtos/MoveListItemDto";

export interface AnalyticsSummary{
    totalMovies:number,
    totalUsers:number,
    totalReviews:number,
    totalViews:number,
    topMoviesByViews:MovieListItem[]
}