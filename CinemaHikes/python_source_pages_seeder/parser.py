import asyncio
import re
from datetime import datetime
import asyncpg
from urllib.parse import urlparse
from playwright.async_api import async_playwright
from playwright_stealth import Stealth

DB_DSN = "postgresql://postgres:postgres@localhost:5432/cinemahikes"

START_URL = "https://rezka.ag/?filter=popular&genre=1"
START_PAGE = 1
MAX_PAGES = 5
LINK_PATTERN = r"/films/[a-zA-Z0-9\-_]+/(\d+)-[a-zA-Z0-9\-_]+\.html"

REZKA_IP = "179.43.189.162"

ANUBIS_TIMEOUT_MS = 90000


async def save_movie_to_db(conn, link, title, director):
    try:
        year_match = re.search(r'-(\d{4})(?:-[a-z]+)?\.html$', link)
        release_year = int(year_match.group(1)) if year_match else 0

        async with conn.transaction():
            movie_id = await conn.fetchval('''
                INSERT INTO "Movies" (
                    "RuTitle", "UaTitle", "RuInEngTitle", "Description",
                    "Director", "ReleaseYear", "PosterUrl", "KpRating", "CreatedAt"
                ) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9)
                RETURNING "Id"
            ''', title, title, title, "empty", director, release_year, "empty", 0.0, datetime.utcnow())

            await conn.execute('''
                INSERT INTO "VideoSources" (
                    "MovieId", "ProviderName", "PageUrl", "Priority", "Status"
                ) VALUES ($1, $2, $3, $4, $5)
            ''', movie_id, "Rezka", link, 1, 1)

    except Exception as e:
        print(f"[!] Ошибка при сохранении в БД фильма {title}: {e}")


async def main():
    print("[*] Подключение к базе данных...")
    db_conn = await asyncpg.connect(DB_DSN)

    async with Stealth().use_async(async_playwright()) as p:
        browser = await p.chromium.launch(
            headless=False,
            args=[
                "--disable-blink-features=AutomationControlled",
                "--no-sandbox",
                "--disable-dev-shm-usage",
                f"--host-resolver-rules=MAP rezka.ag {REZKA_IP}",
            ]
        )

        context = await browser.new_context(
            viewport={"width": 1280, "height": 800},
            ignore_https_errors=True
        )
        page = await context.new_page()
        base_domain = "https://rezka.ag"

        for page_num in range(START_PAGE, MAX_PAGES + 1):
            valid_links = set()

            if page_num == 1:
                current_url = START_URL
            else:
                parsed_url = urlparse(START_URL)
                current_url = f"{parsed_url.scheme}://{parsed_url.netloc}/page/{page_num}/?{parsed_url.query}"

            print(f"\n=========================================")
            print(f"[*] ПАРСИНГ СТРАНИЦЫ КАТАЛОГА: {page_num}")
            print(f"=========================================")

            try:
                await page.goto(current_url, timeout=60000, wait_until="domcontentloaded")
                
                await page.wait_for_selector(".b-content__inline_item", timeout=ANUBIS_TIMEOUT_MS)

                hrefs = await page.eval_on_selector_all(
                    ".b-content__inline_item a",
                    "elements => elements.map(el => el.getAttribute('href'))"
                )
                if not hrefs:
                    hrefs = await page.eval_on_selector_all("a", "elements => elements.map(el => el.getAttribute('href'))")

                for href in hrefs:
                    if href and re.search(LINK_PATTERN, href):
                        full_url = href if href.startswith("http") else f"{base_domain}{href}"
                        valid_links.add(full_url)

            except Exception as e:
                print(f"[!] Ошибка при сборе ссылок на странице {page_num}: {e}")
                try:
                    await page.screenshot(path=f"error_page_{page_num}.png")
                    print(f"    Скриншот сохранён: error_page_{page_num}.png")
                except Exception:
                    pass
                continue

            print(f"[+] Найдено фильмов на странице {page_num}: {len(valid_links)}. Начинаем обход и сохранение...")

            for link in sorted(valid_links):
                print(f"  [*] Открываю: {link}")
                try:
                    await page.goto(link, timeout=60000, wait_until="domcontentloaded")

                    await page.wait_for_selector(".b-post__title h1", timeout=ANUBIS_TIMEOUT_MS)

                    title_locator = page.locator(".b-post__title h1")
                    title = await title_locator.inner_text() if await title_locator.count() > 0 else "Неизвестно"
                    title = title.strip()

                    director_locator = page.locator("tr").filter(has_text="Режиссер").locator("td").nth(1)
                    director = await director_locator.inner_text() if await director_locator.count() > 0 else "Неизвестно"
                    director = director.strip()

                    print(f"    [>] Сохраняю в БД: {title} | {director}")

                    await save_movie_to_db(db_conn, link, title, director)

                except Exception as e:
                    print(f"    [!] Ошибка при парсинге фильма {link}: {e}")
                    try:
                        await page.screenshot(path="error_movie.png")
                    except Exception:
                        pass

            print(f"[+] Страница {page_num} полностью обработана и надежно сохранена в БД.")

        await browser.close()
        await db_conn.close()
        print("\n[Готово] Сбор данных полностью завершен.")


if __name__ == "__main__":
    asyncio.run(main())
