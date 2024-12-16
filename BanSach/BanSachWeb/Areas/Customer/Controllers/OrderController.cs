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
                o => o.OrderHeader.ApplicationUserId == userId && !o.IsDeleted,
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
                includeProperties: "OrderHeader,Product");

            if (orderDetail == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var product = orderDetail.Product;
            if (product != null)
            {
                product.Quantity += orderDetail.Count;
                product.SoldCount -= orderDetail.Count;
                _unitOfWork.Product.Update(product);
            }

            orderDetail.OrderHeader.OrderStatus = SD.StatusCancelled;
            _unitOfWork.OrderDetail.Update(orderDetail);

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult DeleteOrder(int orderDetailId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orderDetail = _unitOfWork.OrderDetail.GetFirstOrDefault(
                o => o.Id == orderDetailId && o.OrderHeader.ApplicationUserId == userId,
                includeProperties: "OrderHeader");

            if (orderDetail == null)
            {
                return RedirectToAction(nameof(Index));
            }

            orderDetail.IsDeleted = true;
            _unitOfWork.OrderDetail.Update(orderDetail);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
