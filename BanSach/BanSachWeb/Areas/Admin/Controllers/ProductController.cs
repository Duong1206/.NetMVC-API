using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;
using BanSach.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        //hàm khởi tạo
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index()
        {
            IEnumerable<Product> objcovertypeList = _unitOfWork.Product.GetAll();
            return View(objcovertypeList);
        }
        [Authorize(Roles = "Admin,Employee")]

        [HttpGet]
        public IActionResult Detail(int id)
        {
            ProductVM productVM = new ProductVM();

            productVM.product = new Product();

            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(
              u => new SelectListItem()
              {
                  Text = u.Name,
                  Value = u.Id.ToString()
              });

            productVM.CoverTypeList = _unitOfWork.coverType.GetAll().Select(
              u => new SelectListItem()
              {
                  Text = u.Name,
                  Value = u.Id.ToString()
              });


            if (id == null || id == 0)
            {

                return View(productVM);
            }
            else
            {
                productVM.product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            }


            return View(productVM);
        }



        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM();

            productVM.product = new Product();

            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(
              u => new SelectListItem()
              {
                  Text = u.Name,
                  Value = u.Id.ToString()
              });

            productVM.CoverTypeList = _unitOfWork.coverType.GetAll().Select(
              u => new SelectListItem()
              {
                  Text = u.Name,
                  Value = u.Id.ToString()
              });


            if (id == null || id == 0)
            {

                return View(productVM);
            }
            else
            {
                productVM.product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            }


            return View(productVM);
        }
        // 

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    const long maxFileSize = 10 * 1024 * 1024;
                    if (file.Length > maxFileSize)
                    {
                        TempData["error"] = "File size exceeds 10MB. Please upload a smaller file.";
                        return View(obj);
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["error"] = "Invalid file format. Only .jpg, .jpeg, and .png files are allowed.";
                        return View(obj);
                    }

                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products\");
                    if (obj.product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else if (obj.product.Id == 0) 
                {
                    TempData["error"] = "Please upload an image for the product.";
                    return View(obj);
                }

                if (obj.product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.product);
                    TempData["success"] = "Product Created Successfully";
                }
                else
                {
                    var existingProduct = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == obj.product.Id);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = obj.product.Name;
                        existingProduct.Description = obj.product.Description;
                        existingProduct.ISBN = obj.product.ISBN;
                        existingProduct.Author = obj.product.Author;
                        existingProduct.Price50 = obj.product.Price50;
                        existingProduct.Price100 = obj.product.Price100;
                        existingProduct.Quantity = obj.product.Quantity;
                        existingProduct.CategoryId = obj.product.CategoryId;
                        existingProduct.CoverTypeId = obj.product.CoverTypeId;
                        existingProduct.ImageUrl = obj.product.ImageUrl ?? existingProduct.ImageUrl;

                        _unitOfWork.Product.Update(existingProduct);
                        TempData["success"] = "Product Updated Successfully";
                    }
                    else
                    {
                        TempData["error"] = "Product not found.";
                        return View(obj);
                    }
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(obj);
        }







        // Api get All Product
        #region API_Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"Category,coverType");
            return Json(new { data = productList });
        }

        #endregion


        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            if (obj.ImageUrl != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                var oldImagePath = Path.Combine(wwwRootPath, obj.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }



    }

}
