namespace BanSach.Application.Features.Products.DTOs;

public class ProductDetailDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int ISBN { get; set; }
    public string? Author { get; set; }
    public int Quantity { get; set; }
    public int SoldCount { get; set; }
    public double Price50 { get; set; }
    public double Price100 { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int CoverTypeId { get; set; }
    public string? CoverTypeName { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new();
    public List<ProductListDto> RelatedProducts { get; set; } = new();
}

public class ReviewDto
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
