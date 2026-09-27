import type { SourceStatus } from "../../Enums/SourceStatus";

export interface VideoSourceDto {
    Id:number;
    MovieId:number;
    ProviderName:string;
    PageUrl:string;
    Priority:number;
    Status:SourceStatus;
}