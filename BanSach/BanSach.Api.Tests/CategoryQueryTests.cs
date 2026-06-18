using BanSach.Application.Features.Categories.Queries;
using BanSach.Model;
using BanSach.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BanSach.Api.Tests;

public sealed class CategoryQueryTests
{
    [Fact]
    public async Task Handle_ReturnsProjectedCategoriesFromDatabase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);
        context.Categories.AddRange(
            new Category { Name = "Fiction", DisplayOrder = 1, CreatedDate = DateTime.UtcNow },
            new Category { Name = "History", DisplayOrder = 2, CreatedDate = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var cache = new MemoryCache(new MemoryCacheOptions());
        var handler = new GetCategoriesQueryHandler(context, cache);

        var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
        Assert.Collection(result.Value,
            item => Assert.Equal("Fiction", item.Name),
            item => Assert.Equal("History", item.Name));
    }
}
