namespace BanSach.Application.Features.Products.DTOs;

public class ProductFilterDto
{
    public string SearchTerm { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public int? CoverTypeId { get; set; }
    public double? PriceMin { get; set; }
    public double? PriceMax { get; set; }
    public int? MinRating { get; set; }
    public string SortBy { get; set; } = "name"; // name, price, rating, newest, bestseller
    public bool SortAscending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
