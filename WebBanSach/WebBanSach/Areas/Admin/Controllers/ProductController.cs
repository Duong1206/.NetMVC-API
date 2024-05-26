using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebBanSach.Data;
using WebBanSach.Models;
using WebBanSach.Repository.IRepository;
using WebBanSach.Models.ViewModel;
using Microsoft.AspNetCore.Http;

namespace WebBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objProductList = _unitOfWork.product.GetAll();
            return View(objProductList);
        }
        
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM();
            productVM.product = new Product();
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(
                u=>new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                );
            productVM.CoverTypeList = _unitOfWork.covertype.GetAll().Select(
                u => new SelectListItem
                {
                Text = u.Name,
                Value = u.Id.ToString()
                }
                );
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                //update product
            }
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile file )
        {
            if (ModelState.IsValid)
            {
                //upload images
                string wwwRothPath = _webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRothPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.product.ImageUrl = @"images\products" + fileName + extension;
                }

                _unitOfWork.product.Add(obj.product);
                _unitOfWork.Save();
                TempData["Success"] = "Product create Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ProductFromDb = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);
            if (ProductFromDb == null)
            {
                return NotFound();
            }
            return View(ProductFromDb);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.product.Remove(obj);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        #region
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.product.GetAll(includeProperties:"Category");
            return Json(new {data = productList});
        }
        #endregion
    }
}
