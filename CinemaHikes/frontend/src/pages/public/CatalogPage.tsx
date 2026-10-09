import { Typography } from "antd";
import { useTranslation } from "react-i18next";

export const CatalogPage = () => {
  const { t } = useTranslation();
  return <Typography.Title style={{ color: "#fff" }}>{t("catalog.title")}</Typography.Title>;
};
