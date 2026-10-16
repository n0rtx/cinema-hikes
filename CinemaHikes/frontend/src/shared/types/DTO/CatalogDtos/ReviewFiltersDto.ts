import type { ReviewStatus } from "../../Enums/ReviewStatus";

export interface ReviewFilters {
    status?:ReviewStatus,
    movieId?:number,
    userId?:number,
    page:number,
    pageSize:number
};