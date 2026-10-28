# Cinema Hikes

> A free, ad-free online cinema: movie catalog, online streaming, personal account, and a Telegram bot for downloads. A team pet project by Hikers Team, built with ASP.NET Core and Clean Architecture.

[🌐 Live Demo](https://pirat-tv.online) — Backend is currently offline to avoid API costs; see screenshots. 

| | Link |
|---|---|
| 🌐 Deployed site | https://pirat-tv.online |
| 📘 Code documentation (DocFX) | https://n0rtx.github.io/cinema-hikes/ |

## 📖 About

Cinema Hikes is a modern web platform and Telegram bot for watching and downloading movies for free, **completely ad-free**. Users can search for movies, filter the catalog, stream online with a choice of quality and source, add movies to favorites, and keep a viewing history. Through the Telegram bot, users can search for and download movies for free (as a file or a direct link).

The problem the project solves: existing alternatives (Kinogo, HDRezka, etc.) are overloaded with ads, pop-ups, and clunky interfaces. Cinema Hikes offers a clean, modern Netflix-style UI and several fallback player sources.

## ✨ Features

- Movie catalog with pagination, filtering by genre / year / rating, and search by title
- Movie page with description, rating, and an embedded ad-free video player
- Switching between multiple player sources (fallback sources)
- Registration and authentication (JWT + ASP.NET Core Identity)
- Personal account: favorites and viewing history
- Telegram bot: movie search and download (file up to 2 GB or a direct link)
- Admin panel: CRUD for movies, sources, and users, review moderation, analytics
- Background parsing of direct links via Playwright (Rezka and others)
- Support for both Russian and Ukrainian movie titles

## 🎨 Design

A dark Netflix-style theme (Ant Design ConfigProvider). A modern, concise, and fast interface with no ad banners or pop-ups. Responsive layout for desktop and mobile devices.

## 🏗️ Architecture

```
CinemaHikes.WebApi ──► Application ──► Domain ◄── Infrastructure
        ┆                 ▲                             ▲
        ┆                 │                             ┆
        ┆     TelegramBotDownloader                     ┆
        └┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┘
```

| Project | Responsibility |
|---|---|
| `CinemaHikes.Domain` | Entities, enums, repository interfaces, specifications |
| `CinemaHikes.Application` | Services, DTOs, validators, mappings (AutoMapper) |
| `CinemaHikes.Infrastructure` | `CinemaHikesDbContext` (EF Core), repositories, Identity, Playwright parsers, JWT |
| `CinemaHikes.WebApi` | REST controllers, middleware, DI |
| `CinemaHikes.TelegramBotDownloader` | Telegram bot message/callback handlers |

Patterns: Clean Architecture, Repository, Unit of Work, Specification, Dependency Injection, Service Layer, DTO + Mapper, Middleware.

## 🛠️ Tech Stack

| Layer | Technologies |
|---|---|
| Backend | .NET 10, ASP.NET Core Web API, EF Core 10, Npgsql, ASP.NET Core Identity |
| Database | PostgreSQL 16 (Docker) |
| Frontend | React, Vite, Ant Design, React Router, TanStack Query, Zustand, Axios |
| Parsing | Playwright (headless Chromium + anti-detect) |
| Telegram | Telegram.Bot |
| Tests | NUnit, WebApi and bot integration tests |
| Documentation | Swagger / OpenAPI, SRS |
| Deployment | Docker Compose (PostgreSQL) |

## 🔐 Authentication

JWT Bearer + ASP.NET Core Identity.

- Registration / login via email + password (`POST /api/Auth/register`, `POST /api/Auth/login`)
- Password must be at least 6 characters and is stored as a secure Identity hash
- Roles: regular user and administrator
- Protected personal account and admin endpoints

## 🎬 Product Catalog

Movies are stored in the `Movies` table (Russian / Ukrainian / transliterated titles, year, poster, Kinopoisk rating). Relations:
- `MovieGenres` — genres
- `VideoSources` — player source pages (PageUrl)
- `MovieLinks` — direct video links by quality and voice-over studio
- `Reviews` — reviews with moderation

The background job `RevalidateMovieLinksJob` re-checks "stale" direct links.

## 🐳 Docker

```bash
# Start PostgreSQL
cp .example.env .env          # edit if needed
docker compose up -d
```

The `postgres:16-alpine` service with a volume and healthcheck. Environment variables:
- `POSTGRES_DB` (default `cinemahikes`)
- `POSTGRES_USER` / `POSTGRES_PASSWORD`
- `POSTGRES_PORT` (default `5432`)

## ⚙️ Configuration

| Variable / key | Purpose |
|---|---|
| `ConnectionStrings` / Postgres variables | Database connection |
| `JWT` settings | Secret, issuer, audience, token lifetime |
| Telegram Bot Token | Bot token (not stored in the repository) |
| Playwright settings | User-Agent, viewport, anti-detect profiles |

Secrets are provided only via environment variables / `.env`.

## 🚀 Getting Started

Requires the .NET 10 SDK, Node.js, and Docker.

```bash
git clone https://github.com/n0rtx/cinema-hikes.git
cd cinema-hikes

# 1. Database
cp .example.env .env
docker compose up -d

# 2. Backend
cd CinemaHikes
dotnet restore
dotnet run --project src/CinemaHikes.WebApi

# 3. Frontend (in a separate terminal)
cd CinemaHikes/frontend
npm install
npm run dev
```

Migrations are applied on API startup. Swagger is available at `/swagger`.

The Telegram bot is launched separately from the `CinemaHikes.TelegramBotDownloader` project.

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

- Passwords are stored only as ASP.NET Core Identity hashes
- JWT with role checks
- SQL injection is prevented by EF Core
- XSS — React escapes output
- Secrets only in environment variables
- Rate limiting for the Telegram bot (NFR-03)

## 📚 API

Main endpoint groups:

| Group | Endpoints |
|---|---|
| Auth | `POST /api/Auth/register`, `login` |
| Movies | `GET /api/Movies`, `GET /api/Movies/{id}` |
| Genres | `GET /api/Genres` |
| Favorites | `GET/POST/DELETE /api/Favorites` |
| History | `GET/POST /api/History` |
| Reviews | `GET/POST /api/Reviews` |
| Admin | `AdminMovies`, `AdminSources`, `AdminUsers`, `AdminAnalytics` |

Full documentation is available in Swagger UI when the API is running or on DocFX.

## 🗺️ Roadmap

- [ ] Expand the number of parsing sources
- [ ] Improve anti-detect and Playwright stability
- [ ] CI/CD (GitHub Actions)
- [ ] Caching of popular movies
- [ ] More detailed analytics in the admin panel

## 👨‍💻 Authors

| Member                   | Role |
|--------------------------|---|
| Andrii Volobuiev (n0rtx) | Team Lead, Full Stack, DevOps |
| Egor Tagirov (b1mq)      | Full Stack |

## 📄 License
```
Pet project. All rights remain with the authors; no license for reuse is granted.
```