import type { VideoQuality } from "../../Enums/VideoQuality";
import type { TranslationStudioDto } from "./TranslationStudioDto";
export interface MovieLinkDto {
    Id:number;
    MovieId:number;
    TranslationStudioDto :TranslationStudioDto;
    VideoQuality :VideoQuality;
    Url:string;
    UpdatedAt:number;
}