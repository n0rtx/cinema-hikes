import { axiosClient } from "../../../../shared/api/axiosClient";
import type { AnalyticsSummary } from "../../../../shared/types/DTO/AdminDtos/AnalyticSummaryDto";

export const fetchAnalytics = async():Promise<AnalyticsSummary> => {
    const response = await axiosClient.get<AnalyticsSummary>('/api/AdminAnalytics');
    return response.data;
}