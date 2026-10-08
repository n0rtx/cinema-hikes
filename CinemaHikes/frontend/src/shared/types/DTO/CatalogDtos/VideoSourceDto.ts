import type { SourceStatus } from "../../Enums/SourceStatus";

export interface VideoSourceDto {
  id: number;
  movieId: number;
  providerName: string;
  pageUrl: string;
  priority: number;
  status: SourceStatus;
  parsingSourceConfigId?: number | null;
}