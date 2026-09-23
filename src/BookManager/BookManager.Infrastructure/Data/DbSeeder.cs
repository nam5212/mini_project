using BookManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Infrastructure.Data;

public static class DbSeeder
{
    private static readonly string[] Titles1 = ["Lập Trình", "Thuật Toán", "Kiến Trúc", "Hệ Thống", "Thiết Kế", "Cơ Sở Dữ Liệu", "Bảo Mật", "Trí Tuệ Nhân Tạo", "Phát Triển Web", "Bí Quyết", "Clean Code", "Microservices", "Docker & Kubernetes", "Design Patterns", "Domain-Driven Design"];
    private static readonly string[] Titles2 = ["Cơ Bản", "Nâng Cao", "Toàn Tập", "Thực Chiến", "Cho Người Mới Bắt Đầu", "Ứng Dụng", "Hiện Đại", "Chuyên Sâu", "Tối Ưu Hóa", "Nguyên Lý", "Thực Hành"];
    private static readonly string[] Authors = ["Nguyễn Văn A", "Trần Thị B", "Lê Văn C", "Phạm Minh D", "Hoàng Anh E", "Robert C. Martin", "Martin Fowler", "Donald Knuth", "Andrew Hunt", "Erich Gamma", "Kent Beck", "Linus Torvalds"];
    private static readonly string[] Categories = ["Công Nghệ Thông Tin", "Kinh Tế", "Kỹ Năng Sống", "Văn Học", "Khoa Học", "Ngoại Ngữ", "Tâm Lý Học", "Lịch Sử"];

    public static async Task SeedBooksAsync(AppDbContext context, int count = 1000)
    {
        var random = new Random();
        var books = new List<Book>(count);

        for (int i = 1; i <= count; i++)
        {
            var title = $"{Titles1[random.Next(Titles1.Length)]} {Titles2[random.Next(Titles2.Length)]} Vol.{i}";
            var author = Authors[random.Next(Authors.Length)];
            var price = Math.Round((decimal)(random.NextDouble() * 450000 + 50000), -3);
            var category = Categories[random.Next(Categories.Length)];
            var stock = random.Next(1, 201);

            books.Add(new Book
            {
                Title = title,
                Author = author,
                Price = price,
                Category = category,
                Stock = stock
            });
        }

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();
    }
}
