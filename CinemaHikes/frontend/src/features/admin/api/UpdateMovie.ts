import { axiosClient } from "../../../shared/api/axiosClient";
import type { UpdateMovieDto } from "../../../shared/types/DTO/CatalogDtos/UpdateMovieDto";

export const UpdateMovie = async(movieId:number,dto:UpdateMovieDto): Promise<void> => {
    await axiosClient.post(`/api/AdminMovie/${movieId}`, dto)

}