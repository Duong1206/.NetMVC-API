using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index()
        {
            IEnumerable<Contact> objContactList = _unitOfWork.Contact.GetAll();
            return View(objContactList);
        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Upsert(int? id)
        {
         
            Contact Contact = new Contact();

            if (id == null || id == 0)
            {
                return View(Contact);
            }
            else
            {

                Contact = _unitOfWork.Contact.GetFirstOrDefault(u => u.Id == id);

            }

            return View(Contact);
        }
        // 


        [Authorize(Roles = "Admin,Employee")]
        [HttpPost] 
        [ValidateAntiForgeryToken]  
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
