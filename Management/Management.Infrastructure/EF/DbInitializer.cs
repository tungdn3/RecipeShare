using Management.Core.Entities;

namespace Management.Infrastructure.EF;

public static class DbInitializer
{
    public static void Initialize(ManagementDbContext context)
    {
        if (context.Categories.Any())
        {
            return;   // DB has been seeded
        }

        var categories = new Category[]
        {
            new() { Name = "Appetizers" },
            new() { Name = "Beverages" },
            new() { Name = "Breads" },
            new() { Name = "Breakfast" },
            new() { Name = "Desserts" },
            new() { Name = "Main Dishes" },
            new() { Name = "Salads" },
            new() { Name = "Side Dishes" },
            new() { Name = "Soups" },
            new() { Name = "Vegetarian/Vegan" },
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        var recipes = new Recipe[]
        {
            new Recipe()
            {
                CategoryId = 1,
                CookingMinutes = 5,
                CreatedAt = DateTime.UtcNow,
                Description = "Bạn đang muốn tìm một công thức làm nước ép táo ngon hay bạn đang muốn kết hợp táo với các loại rau quả khác để tạo nên một ly nước ép trái cây thơm ngon, vị lạ miệng hơn thì bài viết này đang dành cho bạn đấy. Còn ngại gì mà không vào bếp cùng mình nào.",
                ImageFileName = "gyy31mol.jpg",
                Ingredients = new System.Collections.ObjectModel.Collection<string>
                {
                    "1 củ cà rốt",
                    "1 quả táo"
                },
                Instructions = "Táo sau khi mua về, rửa sạch dưới vòi nước, sau đó cắt thành từng miếng để vừa với máy ép. Đối với cà rốt, dùng dao gọt bỏ lớp vỏ bên ngoài, rồi cũng cắt thành từng miếng nhỏ. Bật máy ép lên, rồi cho táo và cà rốt vào máy, ép lấy nước. Cho nước ép ra ly và thưởng thức ngay nhé, bạn có thể cho thêm một ít mật ong vào, rồi khuấy đều để mật ong hòa tan.",
                IsPublished = true,
                PreparationMinutes = 10,
                PublishedAt = DateTime.UtcNow,
                Title = "Nước ép cà rốt táo",
                UpdatedAt = DateTime.UtcNow,
                UserId = "auth0|6605389a095367b2377bdd43"
            },
            new Recipe()
            {
                CategoryId = 1,
                CookingMinutes = 5,
                CreatedAt = DateTime.UtcNow,
                Description = "Bạn đang muốn tìm một công thức làm nước ép táo ngon hay bạn đang muốn kết hợp táo với các loại rau quả khác để tạo nên một ly nước ép trái cây thơm ngon, vị lạ miệng hơn thì bài viết này đang dành cho bạn đấy. Còn ngại gì mà không vào bếp cùng mình nào.",
                ImageFileName = "gyy31mol.jpg",
                Ingredients = new System.Collections.ObjectModel.Collection<string>
                {
                    "1 củ cà rốt",
                    "1 quả táo"
                },
                Instructions = "Táo sau khi mua về, rửa sạch dưới vòi nước, sau đó cắt thành từng miếng để vừa với máy ép. Đối với cà rốt, dùng dao gọt bỏ lớp vỏ bên ngoài, rồi cũng cắt thành từng miếng nhỏ. Bật máy ép lên, rồi cho táo và cà rốt vào máy, ép lấy nước. Cho nước ép ra ly và thưởng thức ngay nhé, bạn có thể cho thêm một ít mật ong vào, rồi khuấy đều để mật ong hòa tan.",
                IsPublished = true,
                PreparationMinutes = 10,
                PublishedAt = DateTime.UtcNow,
                Title = "Nước ép cà rốt táo",
                UpdatedAt = DateTime.UtcNow,
                UserId = "auth0|6605389a095367b2377bdd43"
            },
            new Recipe()
            {
                CategoryId = 1,
                CookingMinutes = 5,
                CreatedAt = DateTime.UtcNow,
                Description = "Bạn đang muốn tìm một công thức làm nước ép táo ngon hay bạn đang muốn kết hợp táo với các loại rau quả khác để tạo nên một ly nước ép trái cây thơm ngon, vị lạ miệng hơn thì bài viết này đang dành cho bạn đấy. Còn ngại gì mà không vào bếp cùng mình nào.",
                ImageFileName = "gyy31mol.jpg",
                Ingredients = new System.Collections.ObjectModel.Collection<string>
                {
                    "1 củ cà rốt",
                    "1 quả táo"
                },
                Instructions = "Táo sau khi mua về, rửa sạch dưới vòi nước, sau đó cắt thành từng miếng để vừa với máy ép. Đối với cà rốt, dùng dao gọt bỏ lớp vỏ bên ngoài, rồi cũng cắt thành từng miếng nhỏ. Bật máy ép lên, rồi cho táo và cà rốt vào máy, ép lấy nước. Cho nước ép ra ly và thưởng thức ngay nhé, bạn có thể cho thêm một ít mật ong vào, rồi khuấy đều để mật ong hòa tan.",
                IsPublished = true,
                PreparationMinutes = 10,
                PublishedAt = DateTime.UtcNow,
                Title = "Nước ép cà rốt táo",
                UpdatedAt = DateTime.UtcNow,
                UserId = "auth0|6605389a095367b2377bdd43"
            },
            new Recipe()
            {
                CategoryId = 1,
                CookingMinutes = 5,
                CreatedAt = DateTime.UtcNow,
                Description = "Bạn đang muốn tìm một công thức làm nước ép táo ngon hay bạn đang muốn kết hợp táo với các loại rau quả khác để tạo nên một ly nước ép trái cây thơm ngon, vị lạ miệng hơn thì bài viết này đang dành cho bạn đấy. Còn ngại gì mà không vào bếp cùng mình nào.",
                ImageFileName = "gyy31mol.jpg",
                Ingredients = new System.Collections.ObjectModel.Collection<string>
                {
                    "1 củ cà rốt",
                    "1 quả táo"
                },
                Instructions = "Táo sau khi mua về, rửa sạch dưới vòi nước, sau đó cắt thành từng miếng để vừa với máy ép. Đối với cà rốt, dùng dao gọt bỏ lớp vỏ bên ngoài, rồi cũng cắt thành từng miếng nhỏ. Bật máy ép lên, rồi cho táo và cà rốt vào máy, ép lấy nước. Cho nước ép ra ly và thưởng thức ngay nhé, bạn có thể cho thêm một ít mật ong vào, rồi khuấy đều để mật ong hòa tan.",
                IsPublished = true,
                PreparationMinutes = 10,
                PublishedAt = DateTime.UtcNow,
                Title = "Nước ép cà rốt táo",
                UpdatedAt = DateTime.UtcNow,
                UserId = "auth0|6605389a095367b2377bdd43"
            },
        };

        context.Recipes.AddRange(recipes);
        context.SaveChanges();
    }
}
