-- Script seed 1000 books ngẫu nhiên vào database PostgreSQL

INSERT INTO "Books" ("Title", "Author", "Price", "Category", "Stock")
SELECT 
    (ARRAY[
        'Lập Trình', 'Thuật Toán', 'Kiến Trúc', 'Hệ Thống', 'Thiết Kế',
        'Cơ Sở Dữ Liệu', 'Bảo Mật', 'Trí Tuệ Nhân Tạo', 'Phát Triển Web', 'Bí Quyết',
        'Clean Code', 'Microservices', 'Docker & Kubernetes', 'Design Patterns', 'Domain-Driven Design'
    ])[floor(random() * 15 + 1)] || ' ' || 
    (ARRAY[
        'Cơ Bản', 'Nâng Cao', 'Toàn Tập', 'Thực Chiến', 'Cho Người Mới Bắt Đầu',
        'Ứng Dụng', 'Hiện Đại', 'Chuyên Sâu', 'Tối Ưu Hóa', 'Nguyên Lý', 'Thực Hành'
    ])[floor(random() * 11 + 1)] || ' Vol.' || i AS "Title",

    (ARRAY[
        'Nguyễn Văn A', 'Trần Thị B', 'Lê Văn C', 'Phạm Minh D', 'Hoàng Anh E',
        'Robert C. Martin', 'Martin Fowler', 'Donald Knuth', 'Andrew Hunt', 'Erich Gamma',
        'Kent Beck', 'Linus Torvalds', 'Guido van Rossum', 'James Gosling', 'Bjarne Stroustrup'
    ])[floor(random() * 15 + 1)] AS "Author",

    ROUND(CAST(random() * 450000 + 50000 AS numeric), -3) AS "Price",

    (ARRAY[
        'Công Nghệ Thông Tin', 'Kinh Tế', 'Kỹ Năng Sống', 'Văn Học',
        'Khoa Học', 'Ngoại Ngữ', 'Tâm Lý Học', 'Lịch Sử'
    ])[floor(random() * 8 + 1)] AS "Category",

    floor(random() * 200 + 1)::int AS "Stock"
FROM generate_series(1, 1000) AS i;
