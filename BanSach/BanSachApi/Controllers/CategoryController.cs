using BanSach.DataAcess.Mappers;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model.Dtos.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BanSachApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        //hàm khởi tạo
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var category = _unitOfWork.Category.GetAll().Select(s => s.ToCategoryDto());
            return Ok(category);
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) {
            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category.ToCategoryDto());

        }
        [HttpPost]

        public IActionResult Create([FromBody] CreateCategoryRequestDto createCategoryDto)
        {
            var categoryModel = createCategoryDto.ToCategoryFromCreateDTO();
            _unitOfWork.Category.Add(categoryModel);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(GetById), new { id = categoryModel.Id }, categoryModel.ToCategoryDto());
        }
        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateCategoryRequestDto updateCategoryDto)
        {
            var categoryModel = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (categoryModel == null)
            {
                return NotFound();
            }
            categoryModel.Name = updateCategoryDto.Name;
            categoryModel.DisplayOrder = updateCategoryDto.DisplayOrder;
            categoryModel.CreatedDate = updateCategoryDto.CreatedDate;

            _unitOfWork.Save();
            return Ok(categoryModel.ToCategoryDto());
        }
        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var categoryModel = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if(categoryModel == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(categoryModel);
            _unitOfWork.Save();

            return NoContent();
        }
    } 
}
