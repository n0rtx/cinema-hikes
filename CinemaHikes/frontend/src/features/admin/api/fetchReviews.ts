import { axiosClient } from "../../../shared/api/axiosClient"
import type { ReviewDto } from "../../../shared/types/DTO/CatalogDtos/ReviewDto"
import type { ReviewFilters } from "../../../shared/types/DTO/CatalogDtos/ReviewFiltersDto"


export const fetchReviews = async(spec:ReviewFilters,):Promise<ReviewDto[]> => {
    const {data} = await axiosClient.get("/api/AdminReview",{
        params:{
            status:spec.status,
            movieId:spec.movieId,
            userId:spec.userId,
            page:spec.page,
            pageSize:spec.pageSize
        },

    });
    return data;
}