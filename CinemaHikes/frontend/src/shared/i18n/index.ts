import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

import ru from "./locales/ru/translation.json";
import uk from "./locales/uk/translation.json";

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      ru: { translation: ru },
      uk: { translation: uk },
    },
    fallbackLng: "ru",
    supportedLngs: ["ru", "uk"],
    interpolation: {
      escapeValue: false,
    },
    detection: {
      order: ["localStorage", "navigator"],
      caches: ["localStorage"],
      lookupLocalStorage: "cinema-hikes-lang",
    },
  });

export default i18n;
