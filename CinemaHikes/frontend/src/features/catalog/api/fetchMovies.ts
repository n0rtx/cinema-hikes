import { axiosClient } from "../../../shared/api/axiosClient";

import type{ MovieFilters } from "../../../shared/types/DTO/CatalogDtos/MovieFiltersDto";
import type{ MoviesPage } from "../../../shared/types/DTO/CatalogDtos/MoviesPageDto";
export const fetchMovies= async (f:MovieFilters):Promise<MoviesPage> => {
    const {data} = await axiosClient.get("/api/movies",{
        params: {
            search:f.search || undefined,
            genreId:f.genreId,
            year:f.year,
            minRating:f.minRating,
            sort:f.sort,
            page:f.page,
            pageSize:f.pageSize
        },
    });
    if (Array.isArray(data)) return {items:data,total:data.length};
    return {items:data.items ?? [],total:data.total?? data.totalCount ?? 0};
}