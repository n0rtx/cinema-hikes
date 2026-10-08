import type { ReviewStatus } from "../../Enums/ReviewStatus";

export interface ReviewDto {
  id: number;
  movieId: number;
  userId: number;
  text: string;
  rating: number;
  status: ReviewStatus;
  createdAt: string;
}