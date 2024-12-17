using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model.ViewModel;
using BanSach.Utility;
using ESC_POS_USB_NET.Printer;
using Microsoft.AspNetCore.Mvc;
using System.Text;


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

        public IActionResult Index(int orderPage = 1)
        {
            int pageSize = 10;
            var orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(includeProperties: "Product,OrderHeader");

            var orderVMList = orderHeaders.Select(orderHeader => new OrderVM
            {
                OrderHeader = orderHeader,
                OrderDetail = orderDetails.Where(od => od.OrderId == orderHeader.Id)
            }).ToList();

            var pagedOrders = orderVMList.Skip((orderPage - 1) * pageSize).Take(pageSize).ToList();

            var pagingInfo = new PagingInfo
            {
                CurrentPage = orderPage,
                ItemsPerPage = pageSize,
                TotalItems = orderVMList.Count
            };

            var viewModel = new OrderListViewModel
            {
                Orders = pagedOrders,
                PagingInfo = pagingInfo
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult MarkAsProcessing(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.OrderStatus = SD.StatusInProcess;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Order marked as processing successfully." });
            }
            return Json(new { success = false, message = "Error occurred while updating the order status." });
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
                var orderDetails = _unitOfWork.OrderDetail.GetAll(od => od.OrderId == orderId).ToList();

                foreach (var detail in orderDetails)
                {
                    var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == detail.ProductId);
                    if (product != null)
                    {
                        product.Quantity += detail.Count;
                        _unitOfWork.Product.Update(product);
                    }
                }

                orderHeader.OrderStatus = SD.StatusCancelled;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Order marked as cancelled successfully and quantity returned." });
            }
            return Json(new { success = false, message = "Error occurred while updating the order status." });
        }


        [HttpPost]
        public IActionResult MarkAsRefunded(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId);
            if (orderHeader != null)
            {
                var orderDetails = _unitOfWork.OrderDetail.GetAll(od => od.OrderId == orderId).ToList();

                foreach (var detail in orderDetails)
                {
                    var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == detail.ProductId);
                    if (product != null)
                    {
                        product.Quantity += detail.Count;
                        _unitOfWork.Product.Update(product);
                    }
                }

                orderHeader.OrderStatus = SD.StatusRefunded;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Order marked as refunded successfully and quantity returned." });
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

        // xprinter58
        [HttpPost]
        public IActionResult PrintOrder(int orderId)
        {
            var order = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId, includeProperties: "OrderDetails.Product,ApplicationUser");
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            try
            {
                var printerName = "XP-58";
                var printer = new Printer(printerName);

                StringBuilder receiptContent = new StringBuilder();
                receiptContent.AppendLine("Order Receipt");
                receiptContent.AppendLine("------------------------------");
                receiptContent.AppendLine($"Date: {order.OrderDate:dd/MM/yyyy}");
                receiptContent.AppendLine($"Name: {order.ApplicationUser.Name}");
                receiptContent.AppendLine($"Address: {order.ApplicationUser.StreetAddress}, {order.ApplicationUser.State}, {order.ApplicationUser.City}");
                receiptContent.AppendLine("\nItem            Qty   Price");
                receiptContent.AppendLine("------------------------------");

                foreach (var detail in order.OrderDetails)
                {
                    var productName = detail.Product.Name;
                    var quantity = detail.Count;
                    var price = detail.Price.ToString("N0");

                    receiptContent.AppendLine($"{productName,-16} {quantity,3} {price,7} VND");
                }

                receiptContent.AppendLine("------------------------------");
                receiptContent.AppendLine($"Total: {order.OrderTotal:N0} VND");
                receiptContent.AppendLine($"Status: {order.OrderStatus}");
                receiptContent.AppendLine("\nThank you for purchasing at the EBOOK STORE!");

                printer.AlignCenter();
                printer.BoldMode(receiptContent.ToString());

                printer.FullPaperCut();
                printer.PrintDocument();
                return Json(new { success = true, message = "Order is being printed." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while printing order: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new { success = false, message = $"Failed to print order: {ex.Message}" });
            }
        }




    }

}

