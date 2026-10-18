import { axiosClient } from "../../../../shared/api/axiosClient";

export const DeleteMovie = async(movieId:number): Promise<void> => {
    await axiosClient.delete(`/api/AdminMovie/${movieId}`);
}