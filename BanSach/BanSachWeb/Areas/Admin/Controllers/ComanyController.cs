using BanSach.DataAccess.Repository.IRepository;
using BanSach.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        //t?o 1 bi?n 
        private readonly IUnitOfWork _unitOfWork;

        //t?o môi trý?ng lýu h?nh
        private readonly IWebHostEnvironment _webHostEnvironment;

        //hàm kh?i t?o
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        //hàm kh?i t?o
        /* public ProductController(IUnitOfWork unitOfWork)
         {
             _unitOfWork = unitOfWork;
         }*/
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            //t?o m?t bi?n h?ng dl
            IEnumerable<Company> objCompanyList = _unitOfWork.Company.GetAll();
            return View(objCompanyList);
        }

        // l?y ra  1 ð?i tý?ng v?i id
        [Authorize(Roles = "Admin")]
        public IActionResult Upsert(int? id)
        {

           /* Product product = new Product();
            // l?y ra danhs sách Category, coverType d? vào Id trong product
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(
                u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });


            IEnumerable<SelectListItem> CoverTypeList = _unitOfWork.coverType.GetAll().Select(
                u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

*/
            Company company = new Company();

            if (id == null || id == 0)
            {

                //create Product
                //viewbag
                //ViewBag.CategoryList = CategoryList;
                // viewdata
              //  ViewData["CoverTypeList"] = CoverTypeList;
                return View(company);
            }
            else
            {
                // update


                company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);

            }

            // ki?m id 
            // var categoryFromDb = _db.Categories.Find(id);

            return View(company);
        }
        // 


        [Authorize(Roles = "Admin")]
        [HttpPost] // nh?n dl tu form
        [ValidateAntiForgeryToken]  // ch?ng gi? m?o pt post
        public IActionResult Upsert(Company obj)
        {
            //ti?n hành update
            if (ModelState.IsValid)
            {
                //upload Images
                    
                if (obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                }


                _unitOfWork.Save();
                TempData["success"] = "Company Create Successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }

     


        // Api get All Product
        #region API_Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var CompanyList = _unitOfWork.Company.GetAll();
            return Json(new { data = CompanyList });
        }

        #endregion



        /*   public IActionResult DeletePost(int? id)
           {

               // ki?m ð?i tý?ng theo id
               var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
               if (obj == null)
               {
                   return NotFound();
               }
               else
               {

                   if (obj.ImageUrl != null)
                   {


                       if (obj.ImageUrl != null)
                       {
                           string wwwRootPath = _webHostEnvironment.WebRootPath;
                           // Delete the old image
                           var oldImagePath = Path.Combine(wwwRootPath, obj.ImageUrl.TrimStart('\\'));
                           if (System.IO.File.Exists(oldImagePath))
                           {
                               System.IO.File.Delete(oldImagePath);
                           }
                       }
                   }

                       _unitOfWork.Product.Remove(obj);
                   _unitOfWork.Save();

                   return Json(new { success = true, message = "Delete Successful" });
               }

               return View(obj);

           }*/
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }



    }

}
