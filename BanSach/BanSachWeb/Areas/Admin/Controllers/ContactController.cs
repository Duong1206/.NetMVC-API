using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;
using BanSach.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;


namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        //tạo 1 biến 
        private readonly IUnitOfWork _unitOfWork;

        //tạo môi trường lưu hình
        private readonly IWebHostEnvironment _webHostEnvironment;

        //hàm khởi tạo
        public ContactController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        //hàm khởi tạo
        /* public ProductController(IUnitOfWork unitOfWork)
         {
             _unitOfWork = unitOfWork;
         }*/
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            //tạo một biến hứng dl
            IEnumerable<Contact> objContactList = _unitOfWork.Contact.GetAll();
            return View(objContactList);
        }

        // lấy ra  1 đối tượng với id
        [Authorize(Roles = "Admin")]
        public IActionResult Upsert(int? id)
        {

            /* Product product = new Product();
             // lấy ra danhs sách Category, coverType dự vào Id trong product
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
            Contact Contact = new Contact();

            if (id == null || id == 0)
            {

                //create Product
                //viewbag
                //ViewBag.CategoryList = CategoryList;
                // viewdata
                //  ViewData["CoverTypeList"] = CoverTypeList;
                return View(Contact);
            }
            else
            {
                // update


                Contact = _unitOfWork.Contact.GetFirstOrDefault(u => u.Id == id);

            }

            // kiếm id 
            // var categoryFromDb = _db.Categories.Find(id);

            return View(Contact);
        }
        // 


        [Authorize(Roles = "Admin")]
        [HttpPost] // nhận dl tu form
        [ValidateAntiForgeryToken]  // chống giả mạo pt post
        public IActionResult Upsert(Contact obj)
        {
            //tiến hành update
            if (ModelState.IsValid)
            {
                //upload Images

                if (obj.Id == 0)
                {
                    _unitOfWork.Contact.Add(obj);
                }
                else
                {
                    _unitOfWork.Contact.Update(obj);
                }


                _unitOfWork.Save();
                TempData["success"] = "Contact Create Successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }




        // Api get All Product
        #region API_Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var ContactList = _unitOfWork.Contact.GetAll();
            return Json(new { data = ContactList });
        }

        #endregion



        /*   public IActionResult DeletePost(int? id)
           {

               // kiếm đối tượng theo id
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
            var obj = _unitOfWork.Contact.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Contact.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }



    }

}
