import type { VideoQuality } from "../../Enums/VideoQuality";

export interface CreateMovieLinkDto {
    MovieId:number;
    TranslationStudioId:number;
    VideoQuality : VideoQuality;
    Url:string;
}