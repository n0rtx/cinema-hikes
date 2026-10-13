import asyncio
import re
from datetime import datetime
import asyncpg
from urllib.parse import urlparse
from playwright.async_api import async_playwright
from playwright_stealth import Stealth

DB_DSN = "postgresql://postgres:postgres@localhost:5432/cinemahikes"

START_URL = "https://rezka.ag/?filter=popular&genre=1"
MAX_PAGES = 1
LINK_PATTERN = r"/films/[a-zA-Z0-9\-_]+/(\d+)-[a-zA-Z0-9\-_]+\.html"

async def save_to_db(movies_data):
    print("\n[*] Подключение к БД и сохранение данных...")
    conn = await asyncpg.connect(DB_DSN)

    try:
        for link, title, director in movies_data:
            year_match = re.search(r'-(\d{4})\.html$', link)
            release_year = int(year_match.group(1)) if year_match else 0

            movie_id = await conn.fetchval('''
                                           INSERT INTO "Movies" ("RuTitle", "UaTitle", "RuInEngTitle", "Description",
                                                                 "Director", "ReleaseYear", "PosterUrl", "KpRating",
                                                                 "CreatedAt")
                                           VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9) RETURNING "Id"
                                           ''',
                                           title, title, title, "", director, release_year, "", 0.0, datetime.utcnow())

            await conn.execute('''
                               INSERT INTO "VideoSources" ("MovieId", "ProviderName", "PageUrl", "Priority", "Status")
                               VALUES ($1, $2, $3, $4, $5)
                               ''', movie_id, "Rezka", link, 1, 1)

        print(f"[+] Успешно сохранено {len(movies_data)} записей в БД.")
    except Exception as e:
        print(f"[!] Ошибка при сохранении в БД: {e}")
    finally:
        await conn.close()


async def main():
    valid_links = set()
    movies_data = []

    async with Stealth().use_async(async_playwright()) as p:
        browser = await p.chromium.launch(
            headless=False,
            args=["--disable-blink-features=AutomationControlled", "--no-sandbox"]
        )

        context = await browser.new_context(
            viewport={"width": 1280, "height": 800},
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36"
        )

        page = await context.new_page()
        base_domain = "https://rezka.ag"

        for page_num in range(1, MAX_PAGES + 1):
            if page_num == 1:
                current_url = START_URL
            else:
                parsed_url = urlparse(START_URL)
                current_url = f"{parsed_url.scheme}://{parsed_url.netloc}/page/{page_num}/?{parsed_url.query}"

            print(f"\n[*] Перехожу на страницу {page_num}: {current_url}")

            try:
                await page.goto(current_url, timeout=60000, wait_until="domcontentloaded")
                await page.wait_for_timeout(3000)

                content = await page.content()
                if "Пожалуйста, подождите" in content or "Cloudflare" in content:
                    await page.wait_for_timeout(5000)

                hrefs = await page.eval_on_selector_all(
                    ".b-content__inline_item a",
                    "elements => elements.map(el => el.getAttribute('href'))"
                )

                if not hrefs:
                    hrefs = await page.eval_on_selector_all("a",
                                                            "elements => elements.map(el => el.getAttribute('href'))")

                for href in hrefs:
                    if href and re.search(LINK_PATTERN, href):
                        full_url = href if href.startswith("http") else f"{base_domain}{href}"
                        valid_links.add(full_url)

            except Exception as e:
                print(f"[!] Ошибка при парсинге страницы {page_num}: {e}")
                break

        print(f"\n[+] Сбор ссылок завершен. Всего уникальных: {len(valid_links)}. Начинаем сбор данных...")

        for link in sorted(valid_links):
            print(f"[*] Открываю: {link}")
            try:
                await page.goto(link, timeout=60000, wait_until="domcontentloaded")
                await page.wait_for_timeout(2000)

                title_locator = page.locator(".b-post__title h1")
                title = await title_locator.inner_text() if await title_locator.count() > 0 else "Неизвестно"
                title = title.strip()

                director_locator = page.locator("tr").filter(has_text="Режиссер").locator("td").nth(1)
                director = await director_locator.inner_text() if await director_locator.count() > 0 else "Неизвестно"
                director = director.strip()

                movies_data.append((link, title, director))
                print(f"  [>] {title} | {director}")

            except Exception as e:
                print(f"[!] Ошибка при парсинге фильма {link}: {e}")

        await browser.close()

    if movies_data:
        await save_to_db(movies_data)


if __name__ == "__main__":
    asyncio.run(main())