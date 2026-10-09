import type { TFunction } from "i18next";

interface MovieTitles {
  ruTitle?: string | null;
  uaTitle?: string | null;
}

export function getMovieTitle(
  movie: MovieTitles,
  language: string,
  t?: TFunction,
): string {
  const isUk = language.startsWith("uk");
  const title = isUk
    ? movie.uaTitle || movie.ruTitle
    : movie.ruTitle || movie.uaTitle;

  if (title) return title;
  return t ? t("movie.noTitle") : "Без названия";
}
