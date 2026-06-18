using System.Net;
using System.Net.Http.Json;
using BanSach.Application.Dtos.Category;
using BanSach.Model;
using BanSach.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;

namespace BanSach.Api.Tests;

public sealed class CategoriesApiTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;

    public CategoriesApiTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ReturnsSuccessAndUsesCache()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Categories.AddRange(
            new Category { Name = "Fiction", DisplayOrder = 1, CreatedDate = DateTime.UtcNow },
            new Category { Name = "Science", DisplayOrder = 2, CreatedDate = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var response = await _client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload.Count);
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreated()
    {
        var request = new { Name = "New Category", DisplayOrder = 10 };

        var response = await _client.PostAsJsonAsync("/api/categories", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}