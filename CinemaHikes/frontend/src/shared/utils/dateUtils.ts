import dayjs from "dayjs";

export const formatDate = (value?: string | Date | null) =>
  value ? dayjs(value).format("DD.MM.YYYY") : "";