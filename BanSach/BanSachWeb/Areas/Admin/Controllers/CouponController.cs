
using BanSach.DataAccess.Repository.IRepository;
using BanSach.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        //t?o 1 bi?n 
        private readonly IUnitOfWork _unitOfWork;

        //hàm kh?i t?o
        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index()
        {
            //t?o m?t bi?n h?ng dl
            IEnumerable<Coupon> objcouponList = _unitOfWork.Coupon.GetAll();
            return View(objcouponList);
        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost] // nh?n dl tu form
        [ValidateAntiForgeryToken]  // ch?ng gi? m?o pt post
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Create(Coupon obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Coupon.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Coupon created successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }


        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var couponFromDbFirst = _unitOfWork.Coupon.GetFirstOrDefault(u => u.Id == id);
            if (couponFromDbFirst == null)
            {
                return NotFound();
            }
            return View(couponFromDbFirst);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]  
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Edit(Coupon obj)
        {
            
            if (ModelState.IsValid)
            {
                _unitOfWork.Coupon.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Coupon updated successfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }

        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(u => u.Id == id);
            if (couponFromDb == null)
            {
                return NotFound();
            }
            return View(couponFromDb);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]  
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult DeletePost(int? id)
        {
            var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(u => u.Id == id);
            if (couponFromDb == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.Coupon.Remove(couponFromDb);
                _unitOfWork.Save();
                TempData["success"] = "Coupon Delete successfully";
                return RedirectToAction("index");
            }


        }
    }
}
