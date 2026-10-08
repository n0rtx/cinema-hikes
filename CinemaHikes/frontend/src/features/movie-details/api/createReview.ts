import { axiosClient } from "../../../shared/api/axiosClient";
import type { CreateReviewDto } from "../../../shared/types/DTO/CatalogDtos/CreateReviewDto";

export async function createReview(dto: CreateReviewDto): Promise<void> {
  await axiosClient.post("/api/reviews", dto);
}