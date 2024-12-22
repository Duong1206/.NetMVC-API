using BanSach.DataAcess.Mappers;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model.Dtos.Category;
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
        public async Task<IActionResult> GetAll()
        {
            var category = await _unitOfWork.Category.GetAllAsync();
            var categoryDto = category.Select(s=>s.ToCategoryDto());
            return Ok(category);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id) {
            var category = await _unitOfWork.Category.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category.ToCategoryDto());

        }
        [HttpPost]

        public async Task<IActionResult> Create([FromBody] CreateCategoryRequestDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = createCategoryDto.ToCategoryFromCreateDTO();
            await _unitOfWork.Category.CreateAsync(category);

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category.ToCategoryDto());
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
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var categoryModel = await _unitOfWork.Category.DeleteAsync(id);
            if(categoryModel == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    } 
}
