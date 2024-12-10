using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model.ViewModel;
using BanSach.Utility;

namespace BanSachWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(
                u => u.ApplicationUserId == userId);

            var orderDetails = _unitOfWork.OrderDetail.GetAll(
                o => o.OrderHeader.ApplicationUserId == userId,
                includeProperties: "Product,OrderHeader");

            var orderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                OrderDetail = orderDetails,
            };

            return View(orderVM);
        }
        [HttpPost]
        public IActionResult CancelOrder(int orderDetailId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orderDetail = _unitOfWork.OrderDetail.GetFirstOrDefault(
                o => o.Id == orderDetailId && o.OrderHeader.ApplicationUserId == userId,
                includeProperties: "OrderHeader");

            if (orderDetail == null)
            {
                TempData["Error"] = "Order not found or you don't have permission to cancel it.";
                return RedirectToAction(nameof(Index));
            }

            orderDetail.OrderHeader.OrderStatus = SD.StatusCancelled; // Cập nhật trạng thái
            _unitOfWork.OrderDetail.Update(orderDetail); // Sử dụng phương thức cập nhật
            _unitOfWork.Save();

            TempData["Success"] = "Order item canceled successfully.";
            return RedirectToAction(nameof(Index)); // Reset lại trang
        }


    }
}
