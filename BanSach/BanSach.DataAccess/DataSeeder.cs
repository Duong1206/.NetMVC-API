using BanSach.Persistence.Context;
using BanSach.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BanSach.DataAccess
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Seed Roles
            await SeedRoles();

            // Seed Admin & Customer Users
            var adminUser = await SeedAdminUser();
            var customerUsers = await SeedCustomerUsers();

            // Seed Categories
            var categories = await SeedCategories();

            // Seed CoverTypes
            var coverTypes = await SeedCoverTypes();

            // Seed Companies
            var companies = await SeedCompanies();

            // Update users with companies
            await SeedUserCompanies(customerUsers, companies);

            // Seed Products
            var products = await SeedProducts(categories, coverTypes);

            // Seed Reviews
            await SeedReviews(products, customerUsers);

            // Seed Coupons
            await SeedCoupons();

            // Seed Orders
            var orders = await SeedOrders(adminUser, customerUsers, products);

            // Seed Contacts
            await SeedContacts();

            await _context.SaveChangesAsync();
        }

        private async Task SeedRoles()
        {
            string[] roles = { "Admin", "Customer", "Employee" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task<ApplicationUser> SeedAdminUser()
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@bansach.com",
                Email = "admin@bansach.com",
                EmailConfirmed = true,
                Name = "Quản Lý Admin",
                PhoneNumber = "0901234567",
                StreetAddress = "123 Đường Nguyễn Huệ",
                City = "Hà Nội",
                State = "Hà Nội",
                PostalCode = "100000"
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            return adminUser;
        }

        private async Task<List<ApplicationUser>> SeedCustomerUsers()
        {
            var customers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "customer1@bansach.com",
                    Email = "customer1@bansach.com",
                    EmailConfirmed = true,
                    Name = "Nguyễn Văn A",
                    PhoneNumber = "0901111111",
                    StreetAddress = "45 Đường Lê Lợi",
                    City = "Hà Nội",
                    State = "Hà Nội",
                    PostalCode = "100001"
                },
                new ApplicationUser
                {
                    UserName = "customer2@bansach.com",
                    Email = "customer2@bansach.com",
                    EmailConfirmed = true,
                    Name = "Trần Thị B",
                    PhoneNumber = "0902222222",
                    StreetAddress = "67 Đường Trần Hưng Đạo",
                    City = "Hồ Chí Minh",
                    State = "Hồ Chí Minh",
                    PostalCode = "700001"
                },
                new ApplicationUser
                {
                    UserName = "customer3@bansach.com",
                    Email = "customer3@bansach.com",
                    EmailConfirmed = true,
                    Name = "Phạm Văn C",
                    PhoneNumber = "0903333333",
                    StreetAddress = "89 Đường Võ Văn Kiệt",
                    City = "Đà Nẵng",
                    State = "Đà Nẵng",
                    PostalCode = "550001"
                },
                new ApplicationUser
                {
                    UserName = "customer4@bansach.com",
                    Email = "customer4@bansach.com",
                    EmailConfirmed = true,
                    Name = "Lê Thị D",
                    PhoneNumber = "0904444444",
                    StreetAddress = "12 Đường Nguyễn Thái Sơn",
                    City = "Cần Thơ",
                    State = "Cần Thơ",
                    PostalCode = "900001"
                },
                new ApplicationUser
                {
                    UserName = "customer5@bansach.com",
                    Email = "customer5@bansach.com",
                    EmailConfirmed = true,
                    Name = "Hoàng Văn E",
                    PhoneNumber = "0905555555",
                    StreetAddress = "34 Đường Bạch Đằng",
                    City = "Hải Phòng",
                    State = "Hải Phòng",
                    PostalCode = "180001"
                },
                new ApplicationUser
                {
                    UserName = "customer6@bansach.com",
                    Email = "customer6@bansach.com",
                    EmailConfirmed = true,
                    Name = "Vũ Thị F",
                    PhoneNumber = "0906666666",
                    StreetAddress = "56 Đường Chu Văn An",
                    City = "Huế",
                    State = "Thừa Thiên Huế",
                    PostalCode = "530001"
                },
                new ApplicationUser
                {
                    UserName = "customer7@bansach.com",
                    Email = "customer7@bansach.com",
                    EmailConfirmed = true,
                    Name = "Đỗ Văn G",
                    PhoneNumber = "0907777777",
                    StreetAddress = "78 Đường Trần Phú",
                    City = "Quy Nhơn",
                    State = "Bình Định",
                    PostalCode = "550000"
                },
                new ApplicationUser
                {
                    UserName = "customer8@bansach.com",
                    Email = "customer8@bansach.com",
                    EmailConfirmed = true,
                    Name = "Bùi Thị H",
                    PhoneNumber = "0908888888",
                    StreetAddress = "90 Đường Phan Bội Châu",
                    City = "Nha Trang",
                    State = "Khánh Hòa",
                    PostalCode = "650001"
                },
                new ApplicationUser
                {
                    UserName = "customer9@bansach.com",
                    Email = "customer9@bansach.com",
                    EmailConfirmed = true,
                    Name = "Trương Văn I",
                    PhoneNumber = "0909999999",
                    StreetAddress = "23 Đường Hùng Vương",
                    City = "Vinh",
                    State = "Nghệ An",
                    PostalCode = "460000"
                },
                new ApplicationUser
                {
                    UserName = "customer10@bansach.com",
                    Email = "customer10@bansach.com",
                    EmailConfirmed = true,
                    Name = "Ngô Thị J",
                    PhoneNumber = "0910101010",
                    StreetAddress = "45 Đường Ngô Quyền",
                    City = "Hạ Long",
                    State = "Quảng Ninh",
                    PostalCode = "200001"
                }
            };

            foreach (var customer in customers)
            {
                var result = await _userManager.CreateAsync(customer, "Customer@123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(customer, "Customer");
                }
            }

            return customers;
        }

        private async Task<List<Category>> SeedCategories()
        {
            var categories = new List<Category>
            {
                new Category { Name = "Sách Lập Trình", DisplayOrder = 1, CreatedDate = DateTime.Now.AddDays(-90) },
                new Category { Name = "Sách Kinh Tế", DisplayOrder = 2, CreatedDate = DateTime.Now.AddDays(-85) },
                new Category { Name = "Sách Tâm Lý", DisplayOrder = 3, CreatedDate = DateTime.Now.AddDays(-80) },
                new Category { Name = "Sách Tiểu Thuyết", DisplayOrder = 4, CreatedDate = DateTime.Now.AddDays(-75) },
                new Category { Name = "Sách Kỹ Năng Sống", DisplayOrder = 5, CreatedDate = DateTime.Now.AddDays(-70) },
                new Category { Name = "Sách Nghệ Thuật", DisplayOrder = 6, CreatedDate = DateTime.Now.AddDays(-65) },
                new Category { Name = "Sách Lịch Sử", DisplayOrder = 7, CreatedDate = DateTime.Now.AddDays(-60) },
                new Category { Name = "Sách Triết Học", DisplayOrder = 8, CreatedDate = DateTime.Now.AddDays(-55) },
                new Category { Name = "Sách Ngoại Ngữ", DisplayOrder = 9, CreatedDate = DateTime.Now.AddDays(-50) },
                new Category { Name = "Sách Trẻ Em", DisplayOrder = 10, CreatedDate = DateTime.Now.AddDays(-45) },
                new Category { Name = "Sách Khoa Học", DisplayOrder = 11, CreatedDate = DateTime.Now.AddDays(-40) },
                new Category { Name = "Sách Du Lịch", DisplayOrder = 12, CreatedDate = DateTime.Now.AddDays(-35) },
                new Category { Name = "Sách Nấu Ăn", DisplayOrder = 13, CreatedDate = DateTime.Now.AddDays(-30) },
                new Category { Name = "Sách Phát Triển Bản Thân", DisplayOrder = 14, CreatedDate = DateTime.Now.AddDays(-25) },
                new Category { Name = "Sách Quản Lý", DisplayOrder = 15, CreatedDate = DateTime.Now.AddDays(-20) }
            };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
            return categories;
        }

        private async Task<List<CoverType>> SeedCoverTypes()
        {
            var coverTypes = new List<CoverType>
            {
                new CoverType { Name = "Bìa Cứng" },
                new CoverType { Name = "Bìa Mềm" },
                new CoverType { Name = "Bìa Tấm" },
                new CoverType { Name = "Bìa Có Khóa" }
            };

            _context.CoverTypes.AddRange(coverTypes);
            await _context.SaveChangesAsync();
            return coverTypes;
        }

        private async Task<List<Company>> SeedCompanies()
        {
            var companies = new List<Company>
            {
                new Company
                {
                    Name = "BanSach Hà Nội",
                    StreetAddress = "456 Đường Đinh Tiên Hoàng",
                    City = "Hà Nội",
                    State = "Hà Nội",
                    PostalCode = "100000",
                    PhoneNumber = "0243333333"
                },
                new Company
                {
                    Name = "BanSach Hồ Chí Minh",
                    StreetAddress = "789 Đường Nguyễn Hữu Cảnh",
                    City = "Hồ Chí Minh",
                    State = "Hồ Chí Minh",
                    PostalCode = "700000",
                    PhoneNumber = "0288888888"
                },
                new Company
                {
                    Name = "BanSach Đà Nẵng",
                    StreetAddress = "321 Đường Bạch Đằng",
                    City = "Đà Nẵng",
                    State = "Đà Nẵng",
                    PostalCode = "550000",
                    PhoneNumber = "0236666666"
                },
                new Company
                {
                    Name = "BanSach Cần Thơ",
                    StreetAddress = "654 Đường Hòa Bình",
                    City = "Cần Thơ",
                    State = "Cần Thơ",
                    PostalCode = "900000",
                    PhoneNumber = "0292222222"
                },
                new Company
                {
                    Name = "BanSach Hải Phòng",
                    StreetAddress = "987 Đường Tô Hiệu",
                    City = "Hải Phòng",
                    State = "Hải Phòng",
                    PostalCode = "180000",
                    PhoneNumber = "0225555555"
                }
            };

            _context.Companies.AddRange(companies);
            await _context.SaveChangesAsync();
            return companies;
        }

        private async Task SeedUserCompanies(List<ApplicationUser> customers, List<Company> companies)
        {
            for (int i = 0; i < customers.Count; i++)
            {
                customers[i].CompanyId = companies[i % companies.Count].Id;
            }
            _context.ApplicationUsers.UpdateRange(customers);
            await _context.SaveChangesAsync();
        }

        private async Task<List<Product>> SeedProducts(List<Category> categories, List<CoverType> coverTypes)
        {
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    Description = "Hướng dẫn chi tiết về viết code sạch và dễ hiểu",
                    ISBN = 780132350,  // Original: 9780132350884
                    Author = "Robert C. Martin",
                    Quantity = 50,
                    Price50 = 299000,
                    Price100 = 279000,
                    ImageUrl = "/images/products/clean-code.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Lập Trình").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Cứng").Id,
                    SoldCount = 85
                },
                new Product
                {
                    Name = "The Pragmatic Programmer",
                    Description = "Những kinh nghiệm thực tế từ các lập trình viên giàu kinh nghiệm",
                    ISBN = 780201616,  // Original: 9780201616224
                    Author = "David Thomas, Andrew Hunt",
                    Quantity = 45,
                    Price50 = 289000,
                    Price100 = 269000,
                    ImageUrl = "/images/products/pragmatic-programmer.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Lập Trình").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 62
                },
                new Product
                {
                    Name = "Design Patterns: Elements of Reusable Object-Oriented Software",
                    Description = "Các pattern thiết kế quan trọng trong lập trình hướng đối tượng",
                    ISBN = 780201633,  // Original: 9780201633610
                    Author = "Gang of Four",
                    Quantity = 35,
                    Price50 = 359000,
                    Price100 = 339000,
                    ImageUrl = "/images/products/design-patterns.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Lập Trình").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Cứng").Id,
                    SoldCount = 48
                },
                new Product
                {
                    Name = "Tư Duy Nhanh Và Chậm",
                    Description = "Khám phá những cơ chế tư duy của con người",
                    ISBN = 786866229,  // Original: 9786866229175
                    Author = "Daniel Kahneman",
                    Quantity = 80,
                    Price50 = 199000,
                    Price100 = 179000,
                    ImageUrl = "/images/products/thinking-fast-slow.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Tâm Lý").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 156
                },
                new Product
                {
                    Name = "Những Người Thành Công Không Bao Giờ Bỏ Cuộc",
                    Description = "Câu chuyện truyền cảm hứng về những người thành công",
                    ISBN = 786866141,  // Original: 9786866141123
                    Author = "Norman Vincent Peale",
                    Quantity = 120,
                    Price50 = 129000,
                    Price100 = 109000,
                    ImageUrl = "/images/products/never-give-up.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Phát Triển Bản Thân").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 234
                },
                new Product
                {
                    Name = "Sapiens: Lược Sử Loài Người",
                    Description = "Hành trình của nhân loại từ xưa đến nay",
                    ISBN = 786869512,  // Original: 9786869512860
                    Author = "Yuval Noah Harari",
                    Quantity = 60,
                    Price50 = 189000,
                    Price100 = 169000,
                    ImageUrl = "/images/products/sapiens.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Lịch Sử").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 127
                },
                new Product
                {
                    Name = "Mô Hình Kinh Doanh Bán Hàng Cho Mọi Người",
                    Description = "Cách xây dựng mô hình kinh doanh thành công",
                    ISBN = 786869512,  // Original: 9786869512860
                    Author = "Alexander Osterwalder",
                    Quantity = 55,
                    Price50 = 249000,
                    Price100 = 229000,
                    ImageUrl = "/images/products/business-model.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Kinh Tế").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Tấm").Id,
                    SoldCount = 89
                },
                new Product
                {
                    Name = "Có Hay Không: Hành Trình Của Startup",
                    Description = "Những bài học từ các startup thành công",
                    ISBN = 786869512,  // Original: 9786869512861
                    Author = "Peter Thiel",
                    Quantity = 70,
                    Price50 = 219000,
                    Price100 = 199000,
                    ImageUrl = "/images/products/zero-to-one.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Kinh Tế").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Cứng").Id,
                    SoldCount = 103
                },
                new Product
                {
                    Name = "Kỹ Năng Giao Tiếp Hiệu Quả",
                    Description = "Các kỹ năng giao tiếp cần thiết cho thành công",
                    ISBN = 786869512,  // Original: 9786869512862
                    Author = "Dale Carnegie",
                    Quantity = 100,
                    Price50 = 149000,
                    Price100 = 129000,
                    ImageUrl = "/images/products/communication-skills.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Kỹ Năng Sống").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 198
                },
                new Product
                {
                    Name = "Thay Đổi Hay Chết",
                    Description = "Cách thích ứng với sự thay đổi trong kinh doanh",
                    ISBN = 786869512,  // Original: 9786869512863
                    Author = "John Kotter",
                    Quantity = 65,
                    Price50 = 179000,
                    Price100 = 159000,
                    ImageUrl = "/images/products/change-or-die.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Quản Lý").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Cứng").Id,
                    SoldCount = 72
                },
                new Product
                {
                    Name = "Lập Trình Python Cho Người Mới Bắt Đầu",
                    Description = "Hướng dẫn toàn diện về lập trình Python",
                    ISBN = 786869512,  // Original: 9786869512864
                    Author = "Eric Matthes",
                    Quantity = 90,
                    Price50 = 269000,
                    Price100 = 249000,
                    ImageUrl = "/images/products/python-crash-course.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Lập Trình").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 141
                },
                new Product
                {
                    Name = "JavaScript: Những Phần Tốt",
                    Description = "Khám phá những điểm mạnh của JavaScript",
                    ISBN = 786869512,  // Original: 9786869512865
                    Author = "Douglas Crockford",
                    Quantity = 75,
                    Price50 = 239000,
                    Price100 = 219000,
                    ImageUrl = "/images/products/js-good-parts.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Lập Trình").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 96
                },
                new Product
                {
                    Name = "Tiểu Thuyết: Tìm Kiếm Ý Nghĩa",
                    Description = "Hành trình tìm kiếm ý nghĩa trong cuộc sống",
                    ISBN = 786869512,  // Original: 9786869512866
                    Author = "Haruki Murakami",
                    Quantity = 85,
                    Price50 = 159000,
                    Price100 = 139000,
                    ImageUrl = "/images/products/norwegian-wood.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Tiểu Thuyết").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 64
                },
                new Product
                {
                    Name = "Nghệ Thuật Kỹ Thuật Số",
                    Description = "Sự kết hợp giữa công nghệ và nghệ thuật",
                    ISBN = 786869512,  // Original: 9786869512867
                    Author = "Lev Manovich",
                    Quantity = 40,
                    Price50 = 349000,
                    Price100 = 319000,
                    ImageUrl = "/images/products/digital-art.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Nghệ Thuật").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Cứng").Id,
                    SoldCount = 35
                },
                new Product
                {
                    Name = "Du Lịch Thế Giới Trong 80 Ngày",
                    Description = "Hướng dẫn chi tiết cho những du khách muốn khám phá",
                    ISBN = 786869512,  // Original: 9786869512868
                    Author = "Jules Verne",
                    Quantity = 95,
                    Price50 = 169000,
                    Price100 = 149000,
                    ImageUrl = "/images/products/around-world.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Du Lịch").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 118
                },
                new Product
                {
                    Name = "Những Công Thức Nấu Ăn Nổi Tiếng Thế Giới",
                    Description = "Hướng dẫn chi tiết nấu ăn các món ăn nổi tiếng",
                    ISBN = 786869512,  // Original: 9786869512869
                    Author = "Gordon Ramsay",
                    Quantity = 70,
                    Price50 = 189000,
                    Price100 = 169000,
                    ImageUrl = "/images/products/cooking-recipes.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Nấu Ăn").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Cứng").Id,
                    SoldCount = 87
                },
                new Product
                {
                    Name = "Học Tiếng Anh Một Cách Hiệu Quả",
                    Description = "Phương pháp học tiếng Anh nhanh chóng",
                    ISBN = 786869512,  // Original: 9786869512870
                    Author = "Stephen Krashen",
                    Quantity = 110,
                    Price50 = 139000,
                    Price100 = 119000,
                    ImageUrl = "/images/products/english-learning.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Ngoại Ngữ").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Mềm").Id,
                    SoldCount = 176
                },
                new Product
                {
                    Name = "Chuyện Cổ Tích Cho Trẻ Em Thế Giới",
                    Description = "Những câu chuyện cổ tích từ khắp nơi trên thế giới",
                    ISBN = 786869512,  // Original: 9786869512871
                    Author = "Brothers Grimm",
                    Quantity = 200,
                    Price50 = 89000,
                    Price100 = 79000,
                    ImageUrl = "/images/products/fairy-tales.jpg",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sách Trẻ Em").Id,
                    CoverTypeId = coverTypes.FirstOrDefault(ct => ct.Name == "Bìa Cứng").Id,
                    SoldCount = 312
                }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
            return products;
        }

        private async Task SeedReviews(List<Product> products, List<ApplicationUser> users)
        {
            var reviews = new List<Review>();
            var comments = new[]
            {
                "Cuốn sách rất hay và hữu ích, tôi đã học được rất nhiều",
                "Nội dung chi tiết, dễ hiểu, khuyên mọi người đọc",
                "Tuyệt vời! Giúp tôi nâng cao kỹ năng của mình",
                "Rất thích cách tác giả giải thích, rất dễ hiểu",
                "Đáng giá tiền, có thông tin quý báu",
                "Chất lượng bìa sách rất tốt, in ấn đẹp",
                "Sách hay nhưng cũng có phần hơi dài",
                "Rất kinh điển, phải đọc",
                "Giúp tôi hiểu rõ hơn về chủ đề",
                "Sách tuyệt vời, nhất định phải có trong tủ sách"
            };

            var random = new Random();
            for (int i = 0; i < products.Count; i++)
            {
                var numReviews = random.Next(3, 8);
                for (int j = 0; j < numReviews; j++)
                {
                    var review = new Review
                    {
                        ProductId = products[i].Id,
                        ApplicationUserId = users[random.Next(users.Count)].Id,
                        Rating = random.Next(3, 6),
                        Comment = comments[random.Next(comments.Length)],
                        CreatedAt = DateTime.Now.AddDays(-random.Next(1, 60))
                    };
                    reviews.Add(review);
                }
            }

            _context.Reviews.AddRange(reviews);
            await _context.SaveChangesAsync();
        }

        private async Task SeedCoupons()
        {
            var coupons = new List<Coupon>
            {
                new Coupon
                {
                    Name = "SUMMER20",
                    Description = "Giảm 20% cho tất cả sách mùa hè",
                    DateStarted = DateTime.Now.AddDays(-5),
                    DateExpired = DateTime.Now.AddDays(25),
                    Quantity = 1000,
                    DiscountPercentage = 20,
                    Status = 1
                },
                new Coupon
                {
                    Name = "WELCOME10",
                    Description = "Giảm 10% cho khách hàng mới",
                    DateStarted = DateTime.Now.AddDays(-30),
                    DateExpired = DateTime.Now.AddDays(60),
                    Quantity = 5000,
                    DiscountPercentage = 10,
                    Status = 1
                },
                new Coupon
                {
                    Name = "FLASH30",
                    Description = "Giảm 30% flash sale cuối tuần",
                    DateStarted = DateTime.Now.AddDays(-1),
                    DateExpired = DateTime.Now.AddDays(2),
                    Quantity = 500,
                    DiscountPercentage = 30,
                    Status = 1
                },
                new Coupon
                {
                    Name = "LOYAL15",
                    Description = "Giảm 15% cho khách hàng thân thiết",
                    DateStarted = DateTime.Now.AddDays(-10),
                    DateExpired = DateTime.Now.AddDays(50),
                    Quantity = 2000,
                    DiscountPercentage = 15,
                    Status = 1
                },
                new Coupon
                {
                    Name = "NEWBOOK25",
                    Description = "Giảm 25% cho sách mới",
                    DateStarted = DateTime.Now.AddDays(-3),
                    DateExpired = DateTime.Now.AddDays(27),
                    Quantity = 800,
                    DiscountPercentage = 25,
                    Status = 1
                },
                new Coupon
                {
                    Name = "BULK50",
                    Description = "Giảm 50% khi mua từ 5 cuốn trở lên",
                    DateStarted = DateTime.Now.AddDays(-15),
                    DateExpired = DateTime.Now.AddDays(45),
                    Quantity = 300,
                    DiscountPercentage = 50,
                    Status = 1
                }
            };

            _context.Coupons.AddRange(coupons);
            await _context.SaveChangesAsync();
        }

        private async Task<List<OrderHeader>> SeedOrders(ApplicationUser adminUser, List<ApplicationUser> customers, List<Product> products)
        {
            var orders = new List<OrderHeader>();
            var orderDetails = new List<OrderDetail>();
            var random = new Random();

            for (int i = 0; i < 12; i++)
            {
                var customer = customers[random.Next(customers.Count)];
                var orderDate = DateTime.Now.AddDays(-random.Next(1, 60));

                var order = new OrderHeader
                {
                    ApplicationUserId = customer.Id,
                    OrderDate = orderDate,
                    ShippingDate = orderDate.AddDays(3),
                    OrderStatus = GetRandomOrderStatus(),
                    PaymentStatus = GetRandomPaymentStatus(),
                    TrackingNumber = $"TRK{random.Next(100000000, 999999999)}",
                    Carrier = new[] { "VNPost", "SPX", "Giao Hàng Nhanh", "Grab Express" }[random.Next(4)],
                    PaymentDate = orderDate.AddDays(1),
                    PaymentDueDate = orderDate.AddDays(7),
                    PhoneNumber = customer.PhoneNumber,
                    StreetAddress = customer.StreetAddress,
                    City = customer.City,
                    State = customer.State,
                    PostalCode = customer.PostalCode,
                    Name = customer.Name
                };

                var numItems = random.Next(1, 5);
                var orderTotal = 0.0;

                for (int j = 0; j < numItems; j++)
                {
                    var product = products[random.Next(products.Count)];
                    var quantity = random.Next(1, 4);
                    var price = product.Price50;

                    var orderDetail = new OrderDetail
                    {
                        ProductId = product.Id,
                        Count = quantity,
                        Price = price,
                        IsDeleted = false
                    };

                    orderDetails.Add(orderDetail);
                    orderTotal += price * quantity;
                }

                order.OrderTotal = orderTotal;
                orders.Add(order);
            }

            _context.OrderHeaders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Set OrderId for details after headers are saved
            for (int i = 0; i < orderDetails.Count; i++)
            {
                orderDetails[i].OrderId = orders[i / 4].Id; // Distribute details across orders
            }

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();

            return orders;
        }

        private async Task SeedContacts()
        {
            var contacts = new List<Contact>
            {
                new Contact
                {
                    Name = "BanSach Hà Nội",
                    Address = "456 Đường Đinh Tiên Hoàng, Hà Nội",
                    Map = "https://maps.google.com/?q=21.0285,105.8537",
                    Email = "hanoi@bansach.com",
                    Phone = "0243333333"
                },
                new Contact
                {
                    Name = "BanSach Hồ Chí Minh",
                    Address = "789 Đường Nguyễn Hữu Cảnh, Hồ Chí Minh",
                    Map = "https://maps.google.com/?q=10.7769,106.6966",
                    Email = "hcm@bansach.com",
                    Phone = "0288888888"
                },
                new Contact
                {
                    Name = "BanSach Đà Nẵng",
                    Address = "321 Đường Bạch Đằng, Đà Nẵng",
                    Map = "https://maps.google.com/?q=16.0544,108.2022",
                    Email = "danang@bansach.com",
                    Phone = "0236666666"
                },
                new Contact
                {
                    Name = "BanSach Cần Thơ",
                    Address = "654 Đường Hòa Bình, Cần Thơ",
                    Map = "https://maps.google.com/?q=10.0379,105.7465",
                    Email = "cantho@bansach.com",
                    Phone = "0292222222"
                },
                new Contact
                {
                    Name = "BanSach Hải Phòng",
                    Address = "987 Đường Tô Hiệu, Hải Phòng",
                    Map = "https://maps.google.com/?q=20.8449,106.6881",
                    Email = "haiphong@bansach.com",
                    Phone = "0225555555"
                }
            };

            _context.Contacts.AddRange(contacts);
            await _context.SaveChangesAsync();
        }

        private string GetRandomOrderStatus()
        {
            return new[] { "Đang xử lý", "Đã gửi", "Đã giao", "Đã hủy" }[new Random().Next(4)];
        }

        private string GetRandomPaymentStatus()
        {
            return new[] { "Chưa thanh toán", "Đã thanh toán", "Hoàn tiền" }[new Random().Next(3)];
        }
    }
}
