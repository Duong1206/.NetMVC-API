using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model.ViewModel;
using BanSach.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(includeProperties: "Product,OrderHeader");

            var orderVMList = orderHeaders.Select(orderHeader => new OrderVM
            {
                OrderHeader = orderHeader,
                OrderDetail = orderDetails.Where(od => od.OrderId == orderHeader.Id)
            }).ToList();

            return View(orderVMList);
        }

        [HttpPost]
        public IActionResult MarkAsShipped(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.OrderStatus = SD.StatusShipped;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Order marked as shipped successfully." });
            }
            return Json(new { success = false, message = "Error occurred while updating the order status." });
        }

        [HttpPost]
        public IActionResult MarkAsCancelled(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.OrderStatus = SD.StatusCancelled;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Order marked as cancelled successfully." });
            }
            return Json(new { success = false, message = "Error occurred while updating the order status." });
        }

        [HttpPost]
        public IActionResult MarkAsRefunded(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.OrderStatus = SD.StatusRefunded;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Order marked as refunded successfully." });
            }
            return Json(new { success = false, message = "Error occurred while updating the order status." });
        }

        [HttpPost]
        public IActionResult DeleteOrder(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId);
            if (orderHeader != null)
            {
                _unitOfWork.OrderHeader.Remove(orderHeader);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Order deleted successfully." });
            }
            return Json(new { success = false, message = "Error occurred while deleting the order." });
        }
    }
}
