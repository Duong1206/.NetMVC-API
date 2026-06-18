using AutoMapper;
using BanSach.Application.Dtos.Category;
using BanSach.Model;

namespace BanSach.Application.Mappings;

public sealed class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}
