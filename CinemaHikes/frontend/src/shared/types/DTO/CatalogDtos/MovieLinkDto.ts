import type { VideoQuality } from "../../Enums/VideoQuality";
import type { TranslationStudioDto } from "./TranslationStudioDto";

export interface MovieLinkDto {
  id: number;
  movieId: number;
  translationStudio: TranslationStudioDto;
  videoQuality: VideoQuality;
  url: string;
  updatedAt: string;
}