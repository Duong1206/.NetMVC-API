using BanSach.Application.Common;
using BanSach.Application.Dtos.Category;
using BanSach.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BanSach.Application.Features.Categories.Queries;

public sealed record GetCategoriesQuery : IRequest<Result<IReadOnlyList<CategoryDto>>>;

public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    private const string CacheKey = "categories:all";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public GetCategoriesQueryHandler(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            entry.Priority = CacheItemPriority.Normal;

            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    DisplayOrder = c.DisplayOrder,
                    CreatedDate = c.CreatedDate
                })
                .ToListAsync(cancellationToken);
        });

        return Result<IReadOnlyList<CategoryDto>>.Success(cached!);
    }
}
