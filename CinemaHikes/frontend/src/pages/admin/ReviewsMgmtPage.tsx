
import {  useQuery } from "@tanstack/react-query"
import { ChangeReviewStatus } from "../../features/admin/api/ChangeReviews"
import { ChangeReviewStatusDto } from "../../shared/types/DTO/AdminDtos/ChangeReviewStatusDto"
import Card from "antd/es/card/Card"
import { ReviewStatus } from "../../shared/types/Enums/ReviewStatus"
import { Typography } from "antd"

const statusColors: Record<ReviewStatus,string> = {
    [ReviewStatus.Rejected]:"red",
    [ReviewStatus.Pending]:"gold",
    [ReviewStatus.Approved]:"green",
};
const {Paragraph,Text} = Typography; 

export const ReviewMgmtPage =() => {
    const dto: ChangeReviewStatusDto = {
        reviewId: number,

    };

    
    const {data,isLoading,isFetching} = useQuery({
        queryFn:["admin-reviews"],
        queryKey:ChangeReviewStatus()
    })
}