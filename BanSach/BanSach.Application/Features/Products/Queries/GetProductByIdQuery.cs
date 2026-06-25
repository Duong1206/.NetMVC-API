using BanSach.Application.Common;
using BanSach.Application.Features.Products.DTOs;
using BanSach.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Application.Features.Products.Queries;

public sealed record GetProductByIdQuery(int ProductId) : IRequest<Result<ProductDetailDto>>;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductDetailDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.coverType)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product == null)
            {
                return Result<ProductDetailDto>.Failure("Product not found");
            }

            // Get related products
            var relatedProducts = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.coverType)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(4)
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

            var result = new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ISBN = product.ISBN,
                Author = product.Author,
                Quantity = product.Quantity,
                SoldCount = product.SoldCount,
                Price50 = product.Price50,
                Price100 = product.Price100,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                CoverTypeId = product.CoverTypeId,
                CoverTypeName = product.coverType?.Name ?? string.Empty,
                AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = product.Reviews.Count(),
                Reviews = product.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        UserName = r.ApplicationUser?.Name ?? "Anonymous",
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToList(),
                RelatedProducts = relatedProducts
            };

            return Result<ProductDetailDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ProductDetailDto>.Failure($"Error retrieving product: {ex.Message}");
        }
    }
}
