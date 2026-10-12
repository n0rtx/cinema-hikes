# Cinema Hikes

> Бесплатный онлайн-кинотеатр без рекламы: каталог фильмов, онлайн-просмотр, личный кабинет и Telegram-бот для скачивания. Курсовой командный проект Hikers Team на ASP.NET Core с Clean Architecture.

[🌐 Live Demo](https://pirat-tv.online)

| | Ссылка |
|---|---|
| 🌐 Задеплоенный сайт | https://pirat-tv.online |
| 📘 Спецификация требований (SRS) | [SRS.md](SRS.md) |

## 📖 About

Cinema Hikes — современная веб-платформа и Telegram-бот для бесплатного просмотра и скачивания фильмов **полностью без рекламы**. Пользователь может искать фильмы, фильтровать каталог, смотреть онлайн с выбором качества и источника, добавлять в избранное и вести историю просмотров. Через Telegram-бота доступен поиск и бесплатное скачивание фильмов (файл или прямая ссылка).

Проблема, которую решает проект: существующие аналоги (Kinogo, HDRezka и др.) перегружены рекламой, всплывающими окнами и неудобным интерфейсом. Cinema Hikes предлагает чистый современный UI в стиле Netflix и несколько резервных источников плеера.

## ✨ Features

- Каталог фильмов с пагинацией, фильтрацией по жанру / году / рейтингу и поиском по названию
- Страница фильма с описанием, рейтингом и встроенным видеоплеером без рекламы
- Переключение между несколькими источниками плеера (резервные источники)
- Регистрация и авторизация (JWT + ASP.NET Core Identity)
- Личный кабинет: избранное и история просмотров
- Telegram-бот: поиск фильмов и скачивание (файл до 2 ГБ или прямая ссылка)
- Админ-панель: CRUD фильмов, источников, пользователей, модерация отзывов, аналитика
- Фоновый парсинг прямых ссылок через Playwright (Rezka и др.)
- Поддержка русского и украинского названий фильмов

## 🎨 Design

Тёмная тема в стиле Netflix (Ant Design ConfigProvider). Современный, лаконичный и быстрый интерфейс без рекламных баннеров и всплывающих окон. Адаптивная вёрстка для десктопа и мобильных устройств.

## 🏗️ Architecture

```
CinemaHikes.WebApi ──► Application ──► Domain ◄── Infrastructure
        ┆                 ▲                             ▲
        ┆                 │                             ┆
        ┆     TelegramBotDownloader                     ┆
        └┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┘
```

| Проект | Ответственность |
|---|---|
| `CinemaHikes.Domain` | Сущности, enum'ы, интерфейсы репозиториев, спецификации. |
| `CinemaHikes.Application` | Сервисы, DTO, валидаторы, маппинги (AutoMapper) |
| `CinemaHikes.Infrastructure` | `CinemaHikesDbContext` (EF Core), репозитории, Identity, Playwright-парсеры, JWT |
| `CinemaHikes.WebApi` | REST-контроллеры, middleware, DI |
| `CinemaHikes.TelegramBotDownloader` | Обработчики сообщений/callback'ов Telegram-бота |

Паттерны: Clean Architecture, Repository, Unit of Work, Specification, Dependency Injection, Service Layer, DTO + Mapper, Middleware.

## 🛠️ Tech Stack

| Слой | Технологии |
|---|---|
| Backend | .NET 10, ASP.NET Core Web API, EF Core 10, Npgsql, ASP.NET Core Identity |
| БД | PostgreSQL 16 (Docker) |
| Frontend | React, Vite, Ant Design, React Router, TanStack Query, Zustand, Axios |
| Парсинг | Playwright (headless Chromium + anti-detect) |
| Telegram | Telegram.Bot |
| Тесты | NUnit, интеграционные тесты WebApi и бота |
| Документация | Swagger / OpenAPI, SRS |
| Деплой | Docker Compose (PostgreSQL) |

## 🔐 Authentication

JWT Bearer + ASP.NET Core Identity.

- Регистрация / вход по email + пароль (`POST /api/Auth/register`, `POST /api/Auth/login`)
- Пароль минимум 6 символов, хранится в виде безопасного хеша Identity
- Роли: обычный пользователь и администратор
- Защищённые эндпоинты личного кабинета и админки

## 🎬 Product Catalog

Фильмы хранятся в таблице `Movies` (русские / украинские / транслит-названия, год, постер, рейтинг КП). Связи:
- `MovieGenres` — жанры
- `VideoSources` — страницы-источники плеера (PageUrl)
- `MovieLinks` — прямые ссылки на видео по качеству и студии озвучки
- `Reviews` — отзывы с модерацией

Фоновая задача `RevalidateMovieLinksJob` перепроверяет «протухшие» прямые ссылки.

## 🐳 Docker

```bash
# Запуск PostgreSQL
cp .example.env .env          # при необходимости отредактируйте
docker compose up -d
```

Сервис `postgres:16-alpine` с volume и healthcheck. Переменные окружения:
- `POSTGRES_DB` (по умолчанию `cinemahikes`)
- `POSTGRES_USER` / `POSTGRES_PASSWORD`
- `POSTGRES_PORT` (по умолчанию `5432`)

## ⚙️ Configuration

| Переменная / ключ | Назначение |
|---|---|
| `ConnectionStrings` / переменные Postgres | Подключение к БД |
| `JWT` настройки | Секрет, issuer, audience, время жизни токена |
| Telegram Bot Token | Токен бота (не хранится в репозитории) |
| Настройки Playwright | User-Agent, viewport, anti-detect профили |

Секреты передаются только через переменные окружения / `.env`.

## 🚀 Getting Started

Нужны .NET 10 SDK, Node.js, Docker.

```bash
git clone https://github.com/n0rtx/cinema-hikes.git
cd cinema-hikes

# 1. База данных
cp .example.env .env
docker compose up -d

# 2. Backend
cd CinemaHikes
dotnet restore
dotnet run --project src/CinemaHikes.WebApi

# 3. Frontend (в отдельном терминале)
cd CinemaHikes/frontend
npm install
npm run dev
```

Миграции применяются при старте API. Swagger доступен по адресу `/swagger`.

Telegram-бот запускается отдельно из проекта `CinemaHikes.TelegramBotDownloader`.

## 📦 Build

```bash
dotnet build CinemaHikes/CinemaHikes.slnx
dotnet test  CinemaHikes/CinemaHikes.slnx
```

## 🧪 Testing

- `CinemaHikes.Domain.UnitTests`
- `CinemaHikes.Application.UnitTests`
- `CinemaHikes.WebApi.IntegrationTests`
- `CinemaHikes.TelegramBot.IntegrationTests`

```bash
dotnet test
```

## 🔒 Security

- Пароли — только хеши ASP.NET Core Identity
- JWT с проверкой ролей
- SQL-инъекции исключены за счёт EF Core
- XSS — React экранирует вывод
- Секреты только в переменных окружения
- Rate-limiting для Telegram-бота (NFR-03)

## 📚 API

Основные группы эндпоинтов:

| Группа | Эндпоинты |
|---|---|
| Auth | `POST /api/Auth/register`, `login` |
| Movies | `GET /api/Movies`, `GET /api/Movies/{id}` |
| Genres | `GET /api/Genres` |
| Favorites | `GET/POST/DELETE /api/Favorites` |
| History | `GET/POST /api/History` |
| Reviews | `GET/POST /api/Reviews` |
| Admin | `AdminMovies`, `AdminSources`, `AdminUsers`, `AdminAnalytics` |

Полная документация — Swagger UI при запуске API.

## 🗺️ Roadmap

- [ ] Расширение числа источников парсинга
- [ ] Улучшение anti-detect и стабильности Playwright
- [ ] CI/CD (GitHub Actions)
- [ ] Кэширование популярных фильмов
- [ ] Более детальная аналитика в админке

## 👨‍💻 Authors

| Участник | Роль |
|---|---|
| Волобуев Андрей (andrvol) | Team Lead, Full Stack |
| Тагиров Егор (b1mq) | Full Stack |

Команда **Hikers Team**. Руководитель проекта: Александр Загоруйко.

## 📄 License
```
Учебный курсовой проект. Все права остаются за авторами; лицензия на повторное использование не предоставляется.
```