import type { ReviewStatus } from "../../Enums/ReviewStatus";

export interface ChangeReviewStatusDto{
    reviewId:number,
    newStatus:ReviewStatus
};