import type { VideoQuality } from "../../Enums/VideoQuality";

export interface CreateMovieLinkDto {
    movieId:number;
    translationStudioId:number;
    videoQuality : VideoQuality;
    url:string;
}