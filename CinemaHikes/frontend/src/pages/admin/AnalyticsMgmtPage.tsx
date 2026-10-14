import { useQuery } from "@tanstack/react-query";
import {
  Typography,
  Card,
  Row,
  Col,
  Statistic,
  Button,
  Table,
  Space,
  Tag,
  Image,
  Skeleton,
} from "antd";
import {
  ReloadOutlined,
  VideoCameraOutlined,
  UserOutlined,
  CommentOutlined,
  EyeOutlined,
  TrophyOutlined,
} from "@ant-design/icons";
import type { ColumnsType } from "antd/es/table";
import type { MovieListItem } from "../../shared/types/DTO/CatalogDtos/MoveListItemDto";
import { fetchAnalytics } from "../../features/admin/api/fetchAnalytic";
const {Title} = Typography;
export const AnalyticsMgmtPage = () => {
  const { data, isLoading, isFetching, refetch } = useQuery({
    queryKey: ["admin-analytics"],
    queryFn: fetchAnalytics,
  });
  const columns: ColumnsType<MovieListItem> = [
    {
      title: "Постер",
      dataIndex: "posterUrl",
      key: "posterUrl",
      width: 70,
      render: (url: string) => (
        <Image
          src={url}
          alt="poster"
          width={45}
          height={65}
          style={{ objectFit: "cover", borderRadius: 4 }}
        />
      ),
    },
    {
      title: "Название",
      dataIndex: "ruTitle",
      key: "ruTitle",
      render: (text: string, record) => (
        <div>
          <strong>{text}</strong>
          {record.ruInEngTitle && (
            <div style={{ color: "#8c8c8c", fontSize: 12 }}>
              {record.ruInEngTitle}
            </div>
          )}
        </div>
      ),
    },
    {
      title: "Год",
      dataIndex: "releaseYear",
      key: "releaseYear",
      width: 90,
    },
    {
      title: "Рейтинг",
      dataIndex: "kpRating",
      key: "kpRating",
      width: 100,
      render: (rating: number) => (
        <Tag color={rating >= 7 ? "green" : "volcano"}>{rating}</Tag>
      ),
    },
  ];
  return (
    <div style={{ padding: 24, minHeight: "100vh" }}>
     
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 24,
        }}
      >
        <Title level={2} style={{ color: "#fff", margin: 0 }}>
          Аналитика системы
        </Title>
        <Button
          type="primary"
          icon={<ReloadOutlined spin={isFetching} />}
          onClick={() => refetch()}
          loading={isFetching}
        >
          Обновить данные
        </Button>
      </div>

     
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card variant={"borderless"}>
            <Skeleton loading={isLoading} active paragraph={{ rows: 1 }}>
              <Statistic
                title="Всего фильмов"
                value={data?.totalMovies ?? 0}
                prefix={<VideoCameraOutlined style={{ color: "#1890ff" }} />}
              />
            </Skeleton>
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card variant={"borderless"}>
            <Skeleton loading={isLoading} active paragraph={{ rows: 1 }}>
              <Statistic
                title="Пользователей"
                value={data?.totalUsers ?? 0}
                prefix={<UserOutlined style={{ color: "#52c41a" }} />}
              />
            </Skeleton>
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card variant={"borderless"}>
            <Skeleton loading={isLoading} active paragraph={{ rows: 1 }}>
              <Statistic
                title="Рецензий / Отзывов"
                value={data?.totalReviews ?? 0}
                prefix={<CommentOutlined style={{ color: "#faad14" }} />}
              />
            </Skeleton>
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card variant={"borderless"}>
            <Skeleton loading={isLoading} active paragraph={{ rows: 1 }}>
              <Statistic
                title="Просмотров"
                value={data?.totalViews ?? 0}
                prefix={<EyeOutlined style={{ color: "#ff4d4f" }} />}
              />
            </Skeleton>
          </Card>
        </Col>
      </Row>

      
      <Card
        title={
          <Space>
            <TrophyOutlined style={{ color: "#faad14" }} />
            <span>Топ фильмов по просмотрам</span>
          </Space>
        }
        
      >
        <Table
          columns={columns}
          dataSource={data?.topMoviesByViews}
          rowKey="id"
          loading={isLoading || isFetching}
          pagination={false}
        />
      </Card>
    </div>
  );
};
