import asyncio
import os
import re
from datetime import datetime
from urllib.parse import urlparse

import asyncpg
from dotenv import load_dotenv
from playwright.async_api import async_playwright
from playwright_stealth import Stealth

load_dotenv()

DB_USER = os.getenv("DB_USER")
DB_PASSWORD = os.getenv("DB_PASSWORD")
DB_NAME = os.getenv("DB_NAME")

DB_DSN = f"postgresql://{DB_USER}:{DB_PASSWORD}@localhost:5432/{DB_NAME}"

START_URL = "https://rezka.ag/?filter=popular&genre=1"
START_PAGE = 1
MAX_PAGES = 1
LINK_PATTERN = r"/films/[a-zA-Z0-9\-_]+/(\d+)-[a-zA-Z0-9\-_]+\.html"

REZKA_IP = "179.43.189.162"

ANUBIS_TIMEOUT_MS = 90000

# Текст страницы с ошибкой (503 и т.п.)
ERROR_RE = re.compile(r"503|Service Unavailable|не отвечает|not responding", re.I)
# Текст кнопки / ссылки «Обновить»
RETRY_RE = re.compile(r"обновить|обновите|refresh|reload|try again|повторить", re.I)


async def goto_with_retry(page, url, ready_selector, max_attempts=10, timeout_ms=ANUBIS_TIMEOUT_MS):
    """Открывает url и ждёт ready_selector. Если вылезла 503 — жмёт «Обновить»."""
    loop = asyncio.get_running_loop()

    for attempt in range(1, max_attempts + 1):
        try:
            resp = await page.goto(url, timeout=60000, wait_until="domcontentloaded")
            if resp and resp.status == 503:
                print(f"    [~] 503 от сервера (попытка {attempt}), жду и пробую снова")
        except Exception as e:
            print(f"    [~] goto упал (попытка {attempt}): {e}")
            await asyncio.sleep(3)
            continue

        deadline = loop.time() + timeout_ms / 1000
        while loop.time() < deadline:
            try:
                # 1) страница нормально загрузилась
                if await page.locator(ready_selector).count() > 0:
                    return True

                # 2) страница с ошибкой -> ищем кнопку/ссылку «Обновить»
                body_text = await page.locator("body").inner_text(timeout=2000)
                if ERROR_RE.search(body_text):
                    retry_btn = (
                        page.get_by_role("link", name=RETRY_RE)
                        .or_(page.get_by_role("button", name=RETRY_RE))
                        .or_(page.get_by_text(RETRY_RE))
                    ).first

                    if await retry_btn.count() > 0:
                        print(f"    [~] Страница с ошибкой, жму «Обновить» (попытка {attempt})")
                        await retry_btn.click(timeout=5000)
                    else:
                        print(f"    [~] Кнопки нет, делаю reload (попытка {attempt})")
                        await page.reload(wait_until="domcontentloaded")

                    await asyncio.sleep(3)  # пауза, чтобы сервер успел очухаться
                    continue

            except Exception:
                # во время навигации/клика контекст страницы может пересоздаваться — это нормально
                pass

            await asyncio.sleep(1)

        print(f"    [~] Таймаут ожидания, пробую заново целиком ({attempt}/{max_attempts})")

    return False


async def save_movie_to_db(conn, link, title, director):
    try:
        year_match = re.search(r'-(\d{4})(?:-[a-z]+)?\.html$', link)
        release_year = int(year_match.group(1)) if year_match else 0

        async with conn.transaction():
            movie_id = await conn.fetchval('''
                                           INSERT INTO "Movies" ("RuTitle", "UaTitle", "RuInEngTitle", "Description",
                                                                 "Director", "ReleaseYear", "PosterUrl", "KpRating",
                                                                 "CreatedAt")
                                           VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9) RETURNING "Id"
                                           ''', title, title, title, "empty", director, release_year, "empty", 0.0,
                                           datetime.utcnow())

            await conn.execute('''
                               INSERT INTO "VideoSources" ("MovieId", "ProviderName", "PageUrl", "Priority", "Status")
                               VALUES ($1, $2, $3, $4, $5)
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
                ok = await goto_with_retry(page, current_url, ".b-content__inline_item")
                if not ok:
                    print(f"[!] Не удалось загрузить страницу {page_num}")
                    continue

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
                    ok = await goto_with_retry(page, link, ".b-post__title h1")
                    if not ok:
                        print(f"    [!] Не удалось открыть {link}")
                        continue

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