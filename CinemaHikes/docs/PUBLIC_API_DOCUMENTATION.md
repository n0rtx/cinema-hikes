# CinemaHikes Public API, Services & Repositories Documentation

This document provides a comprehensive technical reference for all **public methods** and contracts across the WebApi Endpoints, Application/Domain Services, and Data Repositories within `CinemaHikes/src`.

---

## 📑 Table of Contents

1. [API Endpoints (WebApi Controllers)](#1-api-endpoints-webapi-controllers)
   - [AuthController](#authcontroller)
   - [MoviesController](#moviescontroller)
   - [FavoritesController](#favoritescontroller)
   - [GenresController](#genrescontroller)
   - [HistoryController](#historycontroller)
   - [ReviewsController](#reviewscontroller)
2. [Services (Application & Domain)](#2-services-application--domain)
   - [IAuthService / AuthService](#iauthservice--authservice)
   - [IMovieService / MovieService](#imovieservice--movieservice)
   - [IMovieDownloadService / MovieDownloadService](#imowiedownloadservice--moviedownloadservice)
   - [IGenreService / GenreService](#igenreservice--genreservice)
   - [IFavoriteService / FavoriteService](#ifavoriteservice--favoriteservice)
   - [IViewHistoryService / ViewHistoryService](#iviewhistoryservice--viewhistoryservice)
   - [IReviewService / ReviewService](#ireviewservice--reviewservice)
   - [IAdminMovieService / AdminMovieService](#iadminmovieservice--adminmovieservice)
   - [IAdminSourceService / AdminSourceService](#iadminsourceservice--adminsourceservice)
   - [IAdminUserService / AdminUserService](#iadminuserservice--adminuserservice)
   - [IAdminAnalyticsService / AdminAnalyticsService](#iadminanalyticsservice--adminanalyticsservice)
   - [ITelegramSearchService](#itelegramsearchservice)
   - [IJwtTokenService / JwtTokenService](#ijwttokenservice--jwttokenservice)
3. [Repositories & UnitOfWork](#3-repositories--unitofwork)
   - [IRepository&lt;T&gt; / BaseRepository&lt;T&gt;](#irepositoryt--baserepositoryt)
   - [IMovieRepository / MovieRepository](#imovierepository--movierepository)
   - [IGenreRepository / GenreRepository](#igenrerepository--genrerepository)
   - [IVideoSourceRepository / VideoSourceRepository](#ivideosourcerepository--videosourcerepository)
   - [IMovieLinkRepository / MovieLinkRepository](#imovielinkrepository--movielinkrepository)
   - [ITranslationStudioRepository / TranslationStudioRepository](#itranslationstudiorepository--translationstudiorepository)
   - [IReviewRepository / ReviewRepository](#ireviewrepository--reviewrepository)
   - [IParsingSourceRepository / ParsingSourceRepository](#iparsingsourcerepository--parsingsourcerepository)
   - [IUserMovieRelationRepository&lt;T&gt; / UserMovieRelationRepository&lt;T&gt;](#iusermovierelationrepositoryt--usermovierelationrepositoryt)
   - [IUnitOfWork / UnitOfWork](#iunitofwork--unitofwork)

---

## 1. API Endpoints (WebApi Controllers)

### `AuthController`
**Route**: `api/Auth`  
**Description**: Handles user registration, credential login, profile retrieval, password changing, and GitHub OAuth authentication.

#### `RegisterAsync`
- **Method**: `POST api/Auth/register`
- **Authorization**: Anonymous
- **Body**: `RegisterRequestDto (Username, Email, Password)`
- **Response**: `200 OK (AuthResponseDto: Token, Username, Email)` | `400 Bad Request`
- **Description**: Registers a new user account with credentials and returns a signed JWT token.

#### `LoginAsync`
- **Method**: `POST api/Auth/login`
- **Authorization**: Anonymous
- **Body**: `LoginRequestDto (Email, Password)`
- **Response**: `200 OK (AuthResponseDto: Token, Username, Email)` | `401 Unauthorized`
- **Description**: Verifies user email and password and returns a signed JWT token.

#### `GetCurrentUserAsync`
- **Method**: `GET api/Auth/me`
- **Authorization**: Bearer JWT (`[Authorize]`)
- **Response**: `200 OK (UserProfileDto: Id, Username, Email, RegisteredAt)` | `401 Unauthorized`
- **Description**: Reads current user identifier from `ClaimTypes.NameIdentifier` claims and retrieves their profile details.

#### `ChangePasswordAsync`
- **Method**: `POST api/Auth/change-password`
- **Authorization**: Bearer JWT (`[Authorize]`)
- **Body**: `ChangePasswordRequestDto (OldPassword, NewPassword)`
- **Response**: `204 NoContent` | `400 Bad Request` | `401 Unauthorized`
- **Description**: Verifies current password and sets the new password for the authenticated user.

#### `Github`
- **Method**: `POST api/Auth/github`
- **Authorization**: Anonymous (`[AllowAnonymous]`)
- **Body**: `GitHubLoginRequestDto (Code)`
- **Response**: `200 OK (AuthResponseDto: Token, Username, Email)` | `400 Bad Request`
- **Description**: Completes GitHub OAuth authorization flow using the temporary authorization code, creating or linking a local account and issuing a JWT token.

---

### `MoviesController`
**Route**: `api/Movies`  
**Description**: Provides public access to movie listings, multi-parameter filtering, details lookup, and search.

#### `GetPagedAsync`
- **Method**: `GET api/Movies`
- **Query Parameters**:
  - `genreId` (int?): Optional genre identifier filter
  - `year` (int?): Optional release year filter
  - `minRating` (double?): Optional minimum Kinopoisk rating threshold
  - `page` (int, default: 1): 1-based page number
  - `pageSize` (int, default: 20, max: 50): Page item count
- **Response**: `200 OK (List<MovieListItemDto>)`
- **Description**: Retrieves a paginated list of catalog movies matching filter criteria.

#### `GetDetailsAsync`
- **Method**: `GET api/Movies/{id}`
- **Route Parameters**: `id` (int)
- **Response**: `200 OK (MovieDetailsDto)` | `404 NotFound`
- **Description**: Fetches comprehensive movie details including genres, video sources, reviews, and translation studios.

#### `SearchAsync`
- **Method**: `GET api/Movies/search`
- **Query Parameters**:
  - `query` (string, required, min 2 chars): Movie search term
  - `page` (int, default: 1)
  - `pageSize` (int, default: 20, max: 50)
- **Response**: `200 OK (List<MovieListItemDto>)` | `400 Bad Request`
- **Description**: Performs search across movie titles; searches local catalog first, falls back to external metadata API if not found, caching results locally.

---

### `FavoritesController`
**Route**: `api/Favorites`  
**Description**: Manages user's personal favorites list. All endpoints require `[Authorize]`.

#### `GetAllAsync`
- **Method**: `GET api/Favorites`
- **Response**: `200 OK (List<FavoriteDto>)` | `401 Unauthorized`
- **Description**: Retrieves all movies saved in favorites by the current user.

#### `AddAsync`
- **Method**: `POST api/Favorites/{movieId}`
- **Route Parameters**: `movieId` (int)
- **Response**: `204 NoContent` | `401 Unauthorized`
- **Description**: Adds the specified movie to the authenticated user's favorites list.

#### `RemoveAsync`
- **Method**: `DELETE api/Favorites/{movieId}`
- **Route Parameters**: `movieId` (int)
- **Response**: `204 NoContent` | `401 Unauthorized`
- **Description**: Removes the specified movie from the authenticated user's favorites list.

---

### `GenresController`
**Route**: `api/Genres`  
**Description**: Public endpoint for movie genres.

#### `GetAll`
- **Method**: `GET api/Genres`
- **Response**: `200 OK (List<GenreDto>)` (cached for 300 seconds via HTTP Response Cache)
- **Description**: Retrieves all catalog genres with identifiers and names.

---

### `HistoryController`
**Route**: `api/History`  
**Description**: Tracks user viewing history and video playback timestamps. All endpoints require `[Authorize]`.

#### `GetAllAsync`
- **Method**: `GET api/History`
- **Response**: `200 OK (List<ViewHistoryDto>)` | `401 Unauthorized`
- **Description**: Returns all viewing history records with playback progress in seconds for the authenticated user.

#### `UpsertProgressAsync`
- **Method**: `PUT api/History/{movieId}`
- **Route Parameters**: `movieId` (int)
- **Body**: `UpdateProgressRequestDto (ProgressSeconds)`
- **Response**: `204 NoContent` | `401 Unauthorized`
- **Description**: Saves or updates playback position (in seconds) for a movie.

---

### `ReviewsController`
**Route**: `api/Reviews`  
**Description**: Public retrieval and authenticated submission of movie reviews.

#### `GetByMovieAsync`
- **Method**: `GET api/Reviews/movie/{movieId}`
- **Route Parameters**: `movieId` (int)
- **Response**: `200 OK (List<ReviewDto>)`
- **Description**: Retrieves all approved reviews for a given movie.

#### `CreateAsync`
- **Method**: `POST api/Reviews`
- **Authorization**: Bearer JWT (`[Authorize]`)
- **Body**: `CreateReviewDto (MovieId, Text, Rating)`
- **Response**: `201 Created` | `401 Unauthorized`
- **Description**: Submits a new review and rating on behalf of the authenticated user with initial Pending status.

---

## 2. Services (Application & Domain)

### `IAuthService` / `AuthService`
- **`RegisterAsync(RegisterRequestDto request)`**: Registers a new user with ASP.NET Identity, hashes the password, creates user claims, and issues a JWT token.
- **`LoginAsync(LoginRequestDto request)`**: Validates credentials against Identity and returns JWT token and user info.
- **`GetCurrentUserAsync(int userId)`**: Retrieves user profile data (ID, username, email, registered timestamp).
- **`ChangePasswordAsync(int userId, ChangePasswordRequestDto request)`**: Verifies existing password and sets the new password.
- **`LoginWithGitHubAsync(string code, CancellationToken ct)`**: Exchanges GitHub code for access token, fetches profile, creates or finds user, and returns signed JWT token.

### `IMovieService` / `MovieService`
- **`GetPagedAsync(int? genreId, int? year, double? minRating, int page, int pageSize, CancellationToken ct)`**: Queries movies using `MovieFilterSpecification` and maps to `MovieListItemDto`.
- **`SearchAsync(string query, int page, int pageSize, CancellationToken ct)`**: Queries local database via `MovieSearchSpecification`; if empty, queries external metadata client and upserts movies to database.
- **`GetDetailsAsync(int id, CancellationToken ct)`**: Retrieves movie details with genres, sources, reviews, and links.

### `IMovieDownloadService` / `MovieDownloadService`
- **`GetDownloadAsync(int movieId, VideoQuality quality, CancellationToken ct)`**: Resolves downloadable video stream URL or direct file link for a given quality level, caching fresh links for 24 hours.

### `IGenreService` / `GenreService`
- **`GetAllAsync(CancellationToken ct)`**: Retrieves all catalog genres using `EmptySpecification<Genre>` and maps to `GenreDto`.

### `IFavoriteService` / `FavoriteService`
- **`GetByUserIdAsync(int userId, CancellationToken ct)`**: Retrieves all favorite movie relations for user with mapped catalog details.
- **`AddAsync(int userId, int movieId, CancellationToken ct)`**: Creates a new `FavoriteMovie` record if not already present.
- **`RemoveAsync(int userId, int movieId, CancellationToken ct)`**: Deletes an existing `FavoriteMovie` record.

### `IViewHistoryService` / `ViewHistoryService`
- **`GetByUserIdAsync(int userId, CancellationToken ct)`**: Returns viewing history list with progress seconds.
- **`UpsertProgressAsync(int userId, int movieId, int progressSeconds, CancellationToken ct)`**: Updates timestamp and progress if existing, otherwise adds a new `ViewHistoryEntry`.

### `IReviewService` / `ReviewService`
- **`GetByMovieIdAsync(int movieId, CancellationToken ct)`**: Retrieves approved reviews for the movie.
- **`CreateAsync(int userId, int movieId, string text, double rating, CancellationToken ct)`**: Inserts a new review in `ReviewStatus.Pending` state.

### `IAdminMovieService` / `AdminMovieService`
- **`CreateAsync(CreateMovieDto dto, CancellationToken ct)`**: Creates a new `Movie` and binds genre join entries (`MovieGenre`).
- **`UpdateAsync(int id, UpdateMovieDto dto, CancellationToken ct)`**: Updates metadata and resets genre assignments.
- **`DeleteAsync(int id, CancellationToken ct)`**: Removes movie from catalog by identifier.

### `IAdminSourceService` / `AdminSourceService`
- **`GetByMovieIdAsync(int movieId, CancellationToken ct)`**: Returns video streaming sources for movie.
- **`AddSourceAsync(int movieId, string providerName, string pageUrl, short priority, CancellationToken ct)`**: Adds a new video source with Active status.
- **`UpdateStatusAsync(int sourceId, SourceStatus status, CancellationToken ct)`**: Updates source health status (Active, Dead, Checking).

### `IAdminUserService` / `AdminUserService`
- **`GetAllAsync(CancellationToken ct)`**: Returns all users with their roles.
- **`AssignRoleAsync(int userId, string roleName, CancellationToken ct)`**: Replaces user's roles with the specified role.

### `IAdminAnalyticsService` / `AdminAnalyticsService`
- **`GetSummaryAsync(CancellationToken ct)`**: Calculates total movie count, review count, and retrieves top 10 movies.

### `ITelegramSearchService`
- **`SearchAsync(string query, int limit, CancellationToken ct)`**: Queries movies formatted for Telegram bot inline keyboard results.

### `IJwtTokenService` / `JwtTokenService`
- **`GenerateToken(TokenClaimsData tokenClaims)`**: Signs and serializes an HMAC-SHA256 JWT token valid for 7 days containing user ID, username, and email claims.

---

## 3. Repositories & UnitOfWork

### `IRepository<T>` / `BaseRepository<T>`
- **`GetByIdAsync(int id, CancellationToken cancellationToken)`**: Finds entity by primary key ID.
- **`ListAsync(ISpecification<T> spec, CancellationToken cancellationToken)`**: Evaluates specification filters, includes, sorting, and pagination.
- **`CountAsync(ISpecification<T> spec, CancellationToken cancellationToken)`**: Counts matching entities matching specification criteria.
- **`AddAsync(T entity, CancellationToken cancellationToken)`**: Attaches new entity in `Added` state.
- **`Update(T entity)`**: Marks entity as `Modified`.
- **`Remove(T entity)`**: Marks entity as `Deleted`.

### `IMovieRepository` / `MovieRepository`
- **`GetWithDetailsAsync(int id, CancellationToken cancellationToken)`**: Loads movie with `MovieGenres`, `VideoSources`, `MovieLinks`, `TranslationStudio`, and `Reviews`.

### `IGenreRepository` / `GenreRepository`
- **`GetByNameAsync(string name, CancellationToken cancellationToken)`**: Looks up genre by name.

### `IVideoSourceRepository` / `VideoSourceRepository`
- **`GetByMovieIdAsync(int movieId, CancellationToken cancellationToken)`**: Queries video sources for movie ordered by ascending priority.

### `IMovieLinkRepository` / `MovieLinkRepository`
- **`GetByMovieAndQualityAsync(int movieId, VideoQuality quality, int translationStudioId, CancellationToken cancellationToken)`**: Looks up cached direct stream link by composite key.
- **`UpsertAsync(MovieLink movieLink, CancellationToken cancellationToken)`**: Inserts or updates stream URL and refreshed timestamp.

### `ITranslationStudioRepository` / `TranslationStudioRepository`
- **`GetByNameAsync(string name, CancellationToken cancellationToken)`**: Looks up voiceover/translation studio by name.

### `IReviewRepository` / `ReviewRepository`
- **`GetByMovieIdAsync(int movieId, ReviewStatus? status, CancellationToken cancellationToken)`**: Queries reviews for movie filtered by status, ordered descending by creation date.

### `IParsingSourceRepository` / `ParsingSourceRepository`
- **`GetEnabledAsync(CancellationToken cancellationToken)`**: Retrieves all parser source configs where `IsEnabled == true`.

### `IUserMovieRelationRepository<T>` / `UserMovieRelationRepository<T>`
- **`GetByUserIdAsync(int userId, CancellationToken cancellationToken)`**: Returns user-movie relation records including movie and genres.
- **`FindAsync(int userId, int movieId, CancellationToken cancellationToken)`**: Finds relation by user ID and movie ID composite key.
- **`AddAsync(T entity, CancellationToken cancellationToken)`**: Adds new relation.
- **`Remove(T entity)`**: Removes relation.

### `IUnitOfWork` / `UnitOfWork`
- **Properties**: `Movies`, `Genres`, `VideoSources`, `MovieLinks`, `TranslationStudios`, `Reviews`, `ParsingSources`, `FavoriteMovies`, `ViewHistory`.
- **`SaveChangesAsync(CancellationToken cancellationToken)`**: Asynchronously commits all entity changes to the database.
