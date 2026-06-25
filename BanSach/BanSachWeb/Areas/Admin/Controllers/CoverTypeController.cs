using BanSach.DataAccess.Repository.IRepository;
using BanSach.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        //t?o 1 bi?n 
        private readonly IUnitOfWork _unitOfWork;

        //hàm kh?i t?o
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index()
        {
            //t?o m?t bi?n h?ng dl
            IEnumerable<CoverType> objcovertypeList = _unitOfWork.coverType.GetAll();
            return View(objcovertypeList);
        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Create()
        {

            return View();
        }
        [Authorize(Roles = "Admin,Employee")]

        [HttpPost] // nh?n dl tu form
        [ValidateAntiForgeryToken]  // ch?ng gi? m?o pt post
        public IActionResult Create(CoverType obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.coverType.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "CoverType created successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }

        // l?y ra  1 ð?i tý?ng v?i id
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var coverTypeFromDbFirst = _unitOfWork.coverType.GetFirstOrDefault(u => u.Id == id);
            if (coverTypeFromDbFirst == null)
            {
                return NotFound();
            }
            return View(coverTypeFromDbFirst);
        }
        // x? l? Edit
        [Authorize(Roles = "Admin,Employee")]


        [HttpPost] // nh?n dl tu form
        [ValidateAntiForgeryToken]  // ch?ng gi? m?o pt post
        public IActionResult Edit(CoverType obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.coverType.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "CoverType updated successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }

        // l?y ra  1 ð?i tý?ng v?i id
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // ki?m id
            var CoverTypeFromDb = _unitOfWork.coverType.GetFirstOrDefault(u => u.Id == id);
            if (CoverTypeFromDb == null)
            {
                return NotFound();
            }
            return View(CoverTypeFromDb);
        }
        // x? l? Edit
        [Authorize(Roles = "Admin,Employee")]

        [HttpPost] // nh?n dl tu form
        [ValidateAntiForgeryToken]  // ch?ng gi? m?o pt post
        public IActionResult DeletePost(int? id)
        {

            // ki?m ð?i tý?ng theo id
            var CoverTypeFromDb = _unitOfWork.coverType.GetFirstOrDefault(u => u.Id == id);
            if (CoverTypeFromDb == null)
            {
                return NotFound();
            }
            else
            {

                _unitOfWork.coverType.Remove(CoverTypeFromDb);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type Delete successfully";
                return RedirectToAction("index");
            }


        }
    }
}
