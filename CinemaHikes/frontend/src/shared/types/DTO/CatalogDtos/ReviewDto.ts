import type { ReviewStatus } from "../../Enums/ReviewStatus";

export interface ReviewDto {
    Id:number;
    MovieId:number;
    UserId:number;
    Text:string;
    Rating:number;
    Status:ReviewStatus;
    CreatedAt:number;
}