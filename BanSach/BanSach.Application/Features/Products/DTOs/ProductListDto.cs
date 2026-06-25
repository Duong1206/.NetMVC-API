namespace BanSach.Application.Features.Products.DTOs;

public class ProductListDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int ISBN { get; set; }
    public string? Author { get; set; }
    public int Quantity { get; set; }
    public int SoldCount { get; set; }
    public double Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int CoverTypeId { get; set; }
    public string? CoverTypeName { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
