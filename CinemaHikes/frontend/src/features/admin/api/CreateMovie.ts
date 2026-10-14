import { axiosClient } from "../../../shared/api/axiosClient";
import type { CreateMovieDto } from "../../../shared/types/DTO/CatalogDtos/CreateMovieDto";

export const CreateMovie = async (dto:CreateMovieDto): Promise<number> => {
    const response  = await axiosClient.post<number>("api/AdminMovie",dto);
    return response.data;
}