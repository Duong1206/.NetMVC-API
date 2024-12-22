using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

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
            Contact contact = new Contact();

            if (id != null && id != 0)
            {
                contact = _unitOfWork.Contact.GetFirstOrDefault(u => u.Id == id);
                if (contact == null)
                {
                    return NotFound();
                }
            }
            return View(contact);
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Contact obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _unitOfWork.Contact.Add(obj);
                    TempData["success"] = "Contact created successfully!";
                }
                else
                {
                    var existingContact = _unitOfWork.Contact.GetFirstOrDefault(c => c.Id == obj.Id);
                    if (existingContact != null)
                    {
                        existingContact.Name = obj.Name;
                        existingContact.Map = obj.Map;
                        existingContact.Address = obj.Address;
                        existingContact.Email = obj.Email;
                        existingContact.Phone = obj.Phone;
                        _unitOfWork.Contact.Update(existingContact);
                        TempData["success"] = "Contact updated successfully!";
                    }
                    else
                    {
                        return NotFound();
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
