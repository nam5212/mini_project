import os
import random
import sys

# Try importing psycopg2 or psycopg, if not installed provide sql output mode
try:
    import psycopg2
    HAS_PSYCOPG2 = True
except ImportError:
    try:
        import psycopg
        HAS_PSYCOPG2 = True
    except ImportError:
        HAS_PSYCOPG2 = False

# Read environment variables or defaults from .env
DB_NAME = os.getenv("POSTGRES_DB", "bookmanager_db")
DB_USER = os.getenv("POSTGRES_USER", "postgres")
DB_PASS = os.getenv("POSTGRES_PASSWORD", "postgres123")
DB_HOST = os.getenv("POSTGRES_HOST", "localhost")
DB_PORT = os.getenv("POSTGRES_PORT", "5432")

TITLES_1 = ['Lập Trình', 'Thuật Toán', 'Kiến Trúc', 'Hệ Thống', 'Thiết Kế', 'Cơ Sở Dữ Liệu', 'Bảo Mật', 'Trí Tuệ Nhân Tạo', 'Phát Triển Web', 'Bí Quyết', 'Clean Code', 'Microservices', 'Docker & Kubernetes', 'Design Patterns', 'Domain-Driven Design']
TITLES_2 = ['Cơ Bản', 'Nâng Cao', 'Toàn Tập', 'Thực Chiến', 'Cho Người Mới Bắt Đầu', 'Ứng Dụng', 'Hiện Đại', 'Chuyên Sâu', 'Tối Ưu Hóa', 'Nguyên Lý', 'Thực Hành']
AUTHORS = ['Nguyễn Văn A', 'Trần Thị B', 'Lê Văn C', 'Phạm Minh D', 'Hoàng Anh E', 'Robert C. Martin', 'Martin Fowler', 'Donald Knuth', 'Andrew Hunt', 'Erich Gamma', 'Kent Beck', 'Linus Torvalds']
CATEGORIES = ['Công Nghệ Thông Tin', 'Kinh Tế', 'Kỹ Năng Sống', 'Văn Học', 'Khoa Học', 'Ngoại Ngữ', 'Tâm Lý Học', 'Lịch Sử']

def generate_books(count=1000):
    books = []
    for i in range(1, count + 1):
        title = f"{random.choice(TITLES_1)} {random.choice(TITLES_2)} Vol.{i}"
        author = random.choice(AUTHORS)
        price = round(random.randint(50, 500) * 1000, 2)
        category = random.choice(CATEGORIES)
        stock = random.randint(1, 200)
        books.append((title, author, price, category, stock))
    return books

def main():
    count = 1000
    if len(sys.argv) > 1 and sys.argv[1].isdigit():
        count = int(sys.argv[1])

    books = generate_books(count)

    if HAS_PSYCOPG2:
        try:
            print(f"Connecting to database {DB_NAME} at {DB_HOST}:{DB_PORT}...")
            conn = psycopg2.connect(
                dbname=DB_NAME,
                user=DB_USER,
                password=DB_PASS,
                host=DB_HOST,
                port=DB_PORT
            )
            cursor = conn.cursor()

            insert_query = """
            INSERT INTO "Books" ("Title", "Author", "Price", "Category", "Stock")
            VALUES (%s, %s, %s, %s, %s)
            """
            
            cursor.executemany(insert_query, books)
            conn.commit()
            cursor.close()
            conn.close()
            print(f"Successfully inserted {count} books into table 'Books'!")
            return
        except Exception as e:
            print(f"Failed to connect or insert directly to DB: {e}")
            print("Falling back to generating SQL file...")

    # Fallback to generating SQL file
    output_file = "seed_1000_books_generated.sql"
    with open(output_file, "w", encoding="utf-8") as f:
        f.write('-- Auto-generated SQL insert script\n')
        f.write('INSERT INTO "Books" ("Title", "Author", "Price", "Category", "Stock") VALUES\n')
        values_list = []
        for b in books:
            title_esc = b[0].replace("'", "''")
            author_esc = b[1].replace("'", "''")
            category_esc = b[3].replace("'", "''")
            values_list.append(f"('{title_esc}', '{author_esc}', {b[2]}, '{category_esc}', {b[4]})")
        f.write(",\n".join(values_list) + ";\n")
    print(f"Generated SQL file '{output_file}' with {count} book records!")

if __name__ == "__main__":
    main()
