namespace BanSach.Application.Dtos.Category;

public sealed class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime CreatedDate { get; set; }
}
