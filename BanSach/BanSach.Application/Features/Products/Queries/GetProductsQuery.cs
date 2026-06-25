using BanSach.Application.Common;
using BanSach.Application.Features.Products.DTOs;
using BanSach.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Application.Features.Products.Queries;

public sealed record GetProductsQuery : IRequest<Result<PagedResult<ProductListDto>>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public int? CoverTypeId { get; set; }
    public double? PriceMin { get; set; }
    public double? PriceMax { get; set; }
    public int? MinRating { get; set; }
    public string SortBy { get; set; } = "name";
    public bool SortAscending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Products.AsNoTracking();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Author.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId);
            }

            if (request.CoverTypeId.HasValue)
            {
                query = query.Where(p => p.CoverTypeId == request.CoverTypeId);
            }

            if (request.PriceMin.HasValue)
            {
                query = query.Where(p => p.Price50 >= request.PriceMin);
            }

            if (request.PriceMax.HasValue)
            {
                query = query.Where(p => p.Price50 <= request.PriceMax);
            }

            // Count total before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = request.SortBy.ToLower() switch
            {
                "price" => request.SortAscending
                    ? query.OrderBy(p => p.Price50)
                    : query.OrderByDescending(p => p.Price50),
                "rating" => request.SortAscending
                    ? query.OrderBy(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0)
                    : query.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0),
                "newest" => query.OrderByDescending(p => p.Id),
                "bestseller" => query.OrderByDescending(p => p.SoldCount),
                _ => request.SortAscending
                    ? query.OrderBy(p => p.Name)
                    : query.OrderByDescending(p => p.Name)
            };

            // Apply pagination
            var skip = (request.PageNumber - 1) * request.PageSize;
            var products = await query
                .Skip(skip)
                .Take(request.PageSize)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ISBN = p.ISBN,
                    Author = p.Author,
                    Quantity = p.Quantity,
                    SoldCount = p.SoldCount,
                    Price = p.Price50,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    CoverTypeId = p.CoverTypeId,
                    CoverTypeName = p.coverType.Name,
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = p.Reviews.Count()
                })
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<ProductListDto>(products, request.PageNumber, request.PageSize, totalCount);
            return Result<PagedResult<ProductListDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<ProductListDto>>.Failure($"Error retrieving products: {ex.Message}");
        }
    }
}

