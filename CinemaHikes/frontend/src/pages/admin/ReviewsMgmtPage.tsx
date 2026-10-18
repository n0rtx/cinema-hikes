import { useMutation, useQuery } from "@tanstack/react-query";
import { ChangeReviewStatus } from "../../features/admin/api/Reviews/ChangeReviews";
import type { ChangeReviewStatusDto } from "../../shared/types/DTO/AdminDtos/ChangeReviewStatusDto";

import { ReviewStatus } from "../../shared/types/Enums/ReviewStatus";
import { Button, Rate, Segmented, Table, Tag, Typography } from "antd";
import { useState } from "react";
import { fetchReviews } from "../../features/admin/api/Reviews/fetchReviews";
import { message } from "antd";
import { CheckOutlined, CloseOutlined } from "@ant-design/icons";
import type { ReviewDto } from "../../shared/types/DTO/CatalogDtos/ReviewDto";
import { formatDate } from "../../shared/utils/dateUtils";
import Title from "antd/es/typography/Title";
const statusColors: Record<ReviewStatus, string> = {
  [ReviewStatus.Rejected]: "red",
  [ReviewStatus.Pending]: "gold",
  [ReviewStatus.Approved]: "green",
};
const { Paragraph } = Typography;
const PAGE_SIZE = 20;
export const ReviewMgmtPage = () => {
  const [page, setPage] = useState(1);
  const [status, setStatus] = useState<ReviewStatus | undefined>(
    ReviewStatus.Pending,
  );
  const {
    data = [],
    isLoading,
    isFetching,
  } = useQuery({
    queryKey: ["admin-reviews", page, status],
    queryFn: () => fetchReviews({ status, page, pageSize: PAGE_SIZE }),
  });
  const mutation = useMutation({
    mutationFn: (dto: ChangeReviewStatusDto) => ChangeReviewStatus(dto),
    onSuccess: () => {
      message.success("Успешно изменено");
    },
    onError: () => {
      message.error("Не удалось изменить статус");
    },
  });
  const columns = [
    {
      title: "Фильм",
      dataIndex: "movieTitle",
      render: (text: string) => <span style={{ color: "#fff" }}>{text}</span>,
    },
    { title: "Пользователь", dataIndex: "username", width: 140 },
    {
      title: "Рейтинг",
      dataIndex: "rating",
      width: 150,
      render: (v: number) => (
        <Rate disabled value={v} style={{ fontSize: 14, color: "#E50914" }} />
      ),
    },
    {
      title: "Текст",
      dataIndex: "text",
      render: (text: string) => (
        <Paragraph
          style={{ color: "#ccc", margin: 0, maxWidth: 360 }}
          ellipsis={{ rows: 2, expandable: true, symbol: "ещё" }}
        >
          {text}
        </Paragraph>
      ),
    },
    {
      title: "Дата",
      dataIndex: "createdAt",
      width: 110,
      render: (v: string) => formatDate(v),
    },
    {
      title: "Статус",
      dataIndex: "status",
      width: 120,
      render: (s: ReviewStatus) => (
        <Tag color={statusColors[s]}>
          {s === ReviewStatus.Approved
            ? "Одобрен"
            : s === ReviewStatus.Rejected
              ? "Отклонен"
              : "Ожидает"}
        </Tag>
      ),
    },
    {
      title: "",
      key: "actions",
      width: 100,
      render: (_: unknown, record: ReviewDto) => (
        <div style={{ display: "flex", gap: 8 }}>
          <Button
            size="small"
            icon={<CheckOutlined />}
            disabled={record.status === ReviewStatus.Approved}
            loading={mutation.isPending}
            onClick={() =>
              mutation.mutate({
                reviewId: record.id,
                newStatus: ReviewStatus.Approved,
              })
            }
            style={{ color: "#52c41a", borderColor: "#52c41a" }}
          />
          <Button
            size="small"
            danger
            icon={<CloseOutlined />}
            disabled={record.status === ReviewStatus.Rejected}
            loading={mutation.isPending}
            onClick={() =>
              mutation.mutate({
                reviewId: record.id,
                newStatus: ReviewStatus.Rejected,
              })
            }
          />
        </div>
      ),
    },
  ];
  return (
    <div>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 20,
          flexWrap: "wrap",
          gap: 12,
        }}
      >
        <Title level={3} style={{ color: "#fff", margin: 0 }}>
          {"Отзывы пользователей"}
        </Title>
        <Segmented
          value={status ?? "all"}
          onChange={(v) => {
            setStatus(v === "all" ? undefined : (v as ReviewStatus));
            setPage(1);
          }}
          options={[
            { label: "Все", value: "all" },
            {
              label: "На проверке",
              value: ReviewStatus.Pending,
            },
            {
              label: "Принят",
              value: ReviewStatus.Approved,
            },
            {
              label: "Отклонен",
              value: ReviewStatus.Rejected,
            },
          ]}
        />
      </div>

      <Table
        rowKey="id"
        columns={columns}
        dataSource={data}
        loading={isLoading}
        pagination={false}
      />

      <div style={{ display: "flex", justifyContent: "center", marginTop: 20 }}>
        <Button
          disabled={data.length < PAGE_SIZE}
          loading={isFetching}
          onClick={() => setPage((p) => p + 1)}
        >
          {"Загрузить больше"}
        </Button>
      </div>
    </div>
  );
};
