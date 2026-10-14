import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { CreateMovie } from "../../features/admin/api/CreateMovie";
import { UpdateMovie } from "../../features/admin/api/UpdateMovie";
import { fetchMovies } from "../../features/catalog/api/fetchMovies";
import { DeleteMovie } from "../../features/admin/api/DeleteMovie";
import {
  Table,
  Button,
  Input,
  Typography,
  Image,
  Tag,
  Popconfirm,
  message,
  Select,
  Space,
  Modal,
  Form,
  InputNumber,
} from "antd";
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import type { ColumnsType } from "antd/es/table";
import type { CreateMovieDto } from "../../shared/types/DTO/CatalogDtos/CreateMovieDto";
import type { UpdateMovieDto } from "../../shared/types/DTO/CatalogDtos/UpdateMovieDto";
import type { MovieListItem } from "../../shared/types/DTO/CatalogDtos/MoveListItemDto";
import type { GenreDto } from "../../shared/types/DTO/CatalogDtos/GenreDto";
import { fetchGenres } from "../../features/catalog/api/fetchGenrest";

const { Title } = Typography;

export const MoviesMgmtPage = () => {
  const queryClient = useQueryClient();
  const [form] = Form.useForm();

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search, setSearch] = useState("");
  const [editingMovie, setEditingMovie] = useState<MovieListItem | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const { data: genres = [] } = useQuery({
    queryKey: ["genres"],
    queryFn: fetchGenres,
  });
  const { data, isLoading } = useQuery({
    queryKey: ["admin-movies", page, pageSize, search],
    queryFn: () => fetchMovies({ page, pageSize, search }),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => DeleteMovie(id),
    onSuccess: () => {
      message.success("Фильм успешно удален");
      queryClient.invalidateQueries({ queryKey: ["admin-movies"] });
    },
    onError: () => {
      message.error("Не удалось удалить фильм");
    },
  });

  const saveMutation = useMutation({
    mutationFn: async (values: any) => {
      
      const dto: UpdateMovieDto = {
        ruTitle: values.ruTitle,
        uaTitle: values.uaTitle ?? "",
        ruInEngTitle: values.ruInEngTitle ?? "",
        description: values.description ?? "",
        director: values.director ?? "",
        releaseYear: Number(values.releaseYear),
        posterUrl: values.posterUrl,
        kpRating: Number(values.kpRating),
        genreIds: values.genreIds, 
      };

      if (editingMovie) {
        await UpdateMovie(editingMovie.id, dto);
      } else {
        await CreateMovie(dto as CreateMovieDto);
      }
    },
    onSuccess: () => {
      message.success(
        editingMovie ? "Фильм успешно обновлен" : "Фильм успешно создан",
      );
      queryClient.invalidateQueries({ queryKey: ["admin-movies"] });
      handleCloseModal();
    },
    onError: () => {
      message.error(
        editingMovie ? "Не удалось обновить фильм" : "Не удалось создать фильм",
      );
    },
  });

  const handleOpenModal = (movie?: MovieListItem) => {
    if (movie) {
      setEditingMovie(movie);
      form.setFieldsValue({
        ...movie,
        genreIds: movie.genres.map((genre) => genre.id),
      });
    } else {
      setEditingMovie(null);
      form.resetFields();
    }
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingMovie(null);
    form.resetFields();
  };

  const columns: ColumnsType<MovieListItem> = [
    {
      title: "Постер",
      dataIndex: "posterUrl",
      key: "posterUrl",
      width: 80,
      render: (url: string) => (
        <Image
          src={url}
          alt="poster"
          width={50}
          height={70}
          style={{ objectFit: "cover", borderRadius: 4 }}
        />
      ),
    },
    {
      title: "Id",
      dataIndex: "id",
      key: "id",
      render: (text: number) => <strong>{text}</strong>,
    },
    {
      title: "Название рус",
      dataIndex: "ruTitle",
      key: "ruTitle",
      render: (text: string) => <strong>{text}</strong>,
    },

    {
      title: "Название укр",
      dataIndex: "uaTitle",
      key: "uaTitle",
      render: (text: string) => <strong>{text ?? "—"}</strong>,
    },
    {
      title: "Название англ",
      dataIndex: "ruInEngTitle",
      key: "engTitle",
      render: (text: string) => <strong>{text ?? "—"}</strong>,
    },
    {
      title: "Год",
      dataIndex: "releaseYear",
      key: "releaseYear",
      width: 100,
    },
    {
      title: "Жанры",
      dataIndex: "genres",
      key: "genres",
      render: (genres: GenreDto[]) =>
        genres?.length ? (
          <Space size={[4, 4]} wrap>
            {genres.map((genre) => (
              <Tag key={genre.id} color="blue">
                {genre.name}
              </Tag>
            ))}
          </Space>
        ) : (
          "—"
        ),
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
    {
      title: "Действия",
      key: "actions",
      width: 140,
      render: (_, record) => (
        <Space size="middle">
          <Button
            type="text"
            icon={<EditOutlined />}
            onClick={() => handleOpenModal(record)}
          />
          <Popconfirm
            title="Удалить фильм?"
            description="Это действие нельзя отменить"
            onConfirm={() => deleteMutation.mutate(record.id)}
            okText="Да"
            cancelText="Нет"
            okButtonProps={{ danger: true, loading: deleteMutation.isPending }}
          >
            <Button type="text" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ color: "#fff" }}>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 24,
        }}
      >
        <Title level={2} style={{ color: "#fff", margin: 0 }}>
          Управление фильмами
        </Title>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => handleOpenModal()}
        >
          Добавить фильм
        </Button>
      </div>

      <Space style={{ marginBottom: 16 }}>
        <Input
          placeholder="Поиск по названию..."
          prefix={<SearchOutlined />}
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          allowClear
          style={{ width: 300 }}
        />
      </Space>

      <Table
        columns={columns}
        dataSource={data?.items}
        rowKey="id"
        loading={isLoading}
        pagination={{
          current: page,
          pageSize: pageSize,
          total: data?.total,
          onChange: (p, ps) => {
            setPage(p);
            setPageSize(ps);
          },
        }}
      />

      <Modal
        title={editingMovie ? "Редактировать фильм" : "Добавить фильм"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        onOk={() => form.submit()}
        confirmLoading={saveMutation.isPending}
        okText="Сохранить"
        cancelText="Отмена"
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={(values) => saveMutation.mutate(values)}
        >
          <Form.Item
            name="ruTitle"
            label="Название рус"
            rules={[{ required: true, message: "Введите русское название" }]}
          >
            <Input />
          </Form.Item>

          <Form.Item name="uaTitle" label="Название укр"rules={[{ required: true, message: "Введите укр название" }]}>
            <Input />
          </Form.Item>

          <Form.Item name="ruInEngTitle" label="Название  англ" rules={[{ required: true, message: "Введите англ название" }]}>
            <Input />
          </Form.Item>

          <Form.Item name="description" label="Описание фильма " rules={[{ required: true, message: "Введите описание " }]}>
            <Input.TextArea rows={4} maxLength={2000} showCount />
          </Form.Item>

          <Form.Item name="director" label="Режиссер фильма " rules={[{ required: true, message: "Введите режиссера" }]}>
            <Input />
          </Form.Item>

          <Form.Item
            name="kpRating"
            label="Рейтинг фильма"
            rules={[{ required: true, message: "Укажи рейтинг" }]}
          >
            <InputNumber style={{ width: "100%" }} min={1.0} max={10.0} />
          </Form.Item>
          <Form.Item
            name="posterUrl"
            label="Ссылка на постер"
            rules={[{ required: true, message: " URL постера" }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="genreIds"
            label="Жанры"
            rules={[
              {
                required: true,
                type: "array",
                min: 1,
                message: "Выберите хотя бы один жанр",
              },
            ]}
          >
            <Select
              mode="multiple"
              placeholder="Выберите жанры"
              options={genres.map((genre) => ({
                label: genre.name,
                value: genre.id,
              }))}
            />
          </Form.Item>
          <Form.Item
            name="releaseYear"
            label="Год выпуска"
            rules={[{ required: true, message: "Укажи ты  год" }]}
          >
            <InputNumber
              style={{ width: "100%" }}
              min={1895}
              max={new Date().getFullYear() + 5}
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};
