using BanSach.Model;
using BanSach.Model.Dtos.Category;
using BanSach.Model.Dtos.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanSach.DataAcess.Mappers
{
    public static class CategoryMappers
    {
        public static CategoryDto ToCategoryDto(this Category categoryModel)
        {
            return new CategoryDto
            {
                Id = categoryModel.Id,
                Name = categoryModel.Name,
                DisplayOrder = categoryModel.DisplayOrder,
                CreatedDate = categoryModel.CreatedDate,
            };
        }
        public static Category ToCategoryFromCreateDTO(this CreateCategoryRequestDto categoryDto)
        {
            return new Category
            {
                Name = categoryDto.Name,
                DisplayOrder = categoryDto.DisplayOrder,
                CreatedDate = categoryDto.CreatedDate,
            };
        }
    }
}
