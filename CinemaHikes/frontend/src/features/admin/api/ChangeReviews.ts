import { axiosClient } from "../../../shared/api/axiosClient";
import type { ChangeReviewStatusDto } from "../../../shared/types/DTO/AdminDtos/ChangeReviewStatusDto";

export const ChangeReviewStatus = async(dto:ChangeReviewStatusDto):Promise<boolean> => {
    const response = await axiosClient.post<boolean>("/api/AdminReview",dto);
    return response.data;
}