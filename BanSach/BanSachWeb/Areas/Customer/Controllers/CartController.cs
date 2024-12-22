using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;
using BanSach.Model.ViewModel;
using BanSach.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BanSachWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [BindProperties]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public int OrderTotl { get; set; }
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;

        }
        public IActionResult Success()
        {
            return View();
        }
        [Authorize]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price50 * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        [Authorize]

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name ??" ";
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber ?? " ";
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress ?? " ";
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City ?? " ";
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State ?? " ";
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode ?? " ";


            foreach (var cart in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price50 * cart.Count);
            }
            return View(ShoppingCartVM);

        }

        [ActionName("Summary")]
        [Authorize]
        [HttpPost]
        public IActionResult SummaryPOST(string PaymentMethod)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == claim.Value, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Product.Price50 * cart.Count;
            }

            if (PaymentMethod == "COD")
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatisPending;

                _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
                _unitOfWork.Save();

                foreach (var cart in ShoppingCartVM.ListCart)
                {

                    var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == cart.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= cart.Count;
                        product.SoldCount += cart.Count;
                        _unitOfWork.Product.Update(product);
                    }

                    var orderDetail = new OrderDetail
                    {
                        ProductId = cart.ProductId,
                        OrderId = ShoppingCartVM.OrderHeader.Id,
                        Price = cart.Product.Price50,
                        Count = cart.Count,
                    };
                    _unitOfWork.OrderDetail.Add(orderDetail);
                }

                _unitOfWork.Save();
                _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
                _unitOfWork.Save();

                return RedirectToAction("Success", "Cart");
            }
            else if (PaymentMethod == "Stripe")
            {
                var domain = "https://localhost:7022/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"customer/cart/StripeSuccess",
                    CancelUrl = domain + $"customer/cart/StripeCancel",
                };

                foreach (var item in ShoppingCartVM.ListCart)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Product.Price50*100 ),
                            Currency = "EUR",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            },
                        },
                        Quantity = item.Count,
                    });
                }

                var service = new SessionService();
                Session session = service.Create(options);

                TempData["StripeSessionId"] = session.Id;
                TempData["OrderHeader"] = Newtonsoft.Json.JsonConvert.SerializeObject(ShoppingCartVM.OrderHeader);
                TempData["OrderDetails"] = Newtonsoft.Json.JsonConvert.SerializeObject(
                    ShoppingCartVM.ListCart.Select(cart => new OrderDetail
                    {
                        ProductId = cart.ProductId,
                        Price = cart.Product.Price50,
                        Count = cart.Count,
                    }).ToList());

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public async Task<IActionResult> StripeSuccess()
        {
            if (TempData["StripeSessionId"] == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            var sessionId = TempData["StripeSessionId"].ToString();
            var orderHeaderJson = TempData["OrderHeader"].ToString();
            var orderDetailsJson = TempData["OrderDetails"].ToString();

            var orderHeader = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderHeader>(orderHeaderJson);
            var orderDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderDetail>>(orderDetailsJson);

            orderHeader.PaymentStatus = SD.PaymentStatusApproved;
            orderHeader.OrderStatus = SD.StatusApproved;

            _unitOfWork.OrderHeader.Add(orderHeader);
            _unitOfWork.Save();

            foreach (var orderDetail in orderDetails)
            {
                orderDetail.OrderId = orderHeader.Id;

                var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == orderDetail.ProductId);
                if (product != null)
                {
                    product.Quantity -= orderDetail.Count;
                    product.SoldCount += orderDetail.Count;
                    _unitOfWork.Product.Update(product);
                }

                _unitOfWork.OrderDetail.Add(orderDetail);
            }

            _unitOfWork.Save();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCartItems = _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == claim.Value);

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartItems);
            _unitOfWork.Save();

            foreach (var detail in orderDetails)
            {
                if (detail.ProductId != 0)
                {
                    detail.Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == detail.ProductId);
                }
            }
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == orderHeader.ApplicationUserId);
            var userEmail = User.Identity.Name;  
            var emailSubject = "YOUR BILL";
            var htmlMessage = $"<h3>Thank you for your order!</h3>" +
                              $"<p>Your order has been successfully processed. Here are the details:</p>" +
                              $"<p>Phone: {user.PhoneNumber}</p>" +
                              $"<p>Address: {user.StreetAddress},{user.State}, {user.City} </p>"+
                              $"<table><tr><th>Product</th>\t|\t<th>Quantity</th>\t|\t<th>Price</th>\t|\t<th>Total</th></tr>";

            foreach (var detail in orderDetails)
            {
                if (detail.Product != null)
                {
                    htmlMessage += $"<tr><td>{detail.Product.Name}</td>\t|\t<td>{detail.Count}</td>\t|\t<td>{detail.Price}</td>\t|\t<td>{detail.Price * detail.Count}</td></tr>";
                }
                else
                {
                    htmlMessage += $"<tr><td>Product not found</td>\t|\t<td>{detail.Count}</td>\t|\t<td>{detail.Price}</td>\t|\t<td>{detail.Price * detail.Count}</td></tr>";
                }
            }

            htmlMessage += $"</table><p>Total: {orderHeader.OrderTotal}</p>";
            await _emailSender.SendEmailAsync(userEmail, emailSubject, htmlMessage);

            return RedirectToAction("Success", "Cart");
        }




        [HttpGet]
        public IActionResult StripeCancel()
        {
            return RedirectToAction("Index", "Cart");
        }


        public IActionResult Plus(int CartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == CartId, includeProperties: "Product");

            if (cart == null || cart.Product == null)
            {
                return NotFound();
            }

            if (cart.Count < cart.Product.Quantity) 
            {
                _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
                _unitOfWork.Save();
            }
            else
            {
                TempData["Error"] = "OUT OF STOCK";
            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Minus(int CartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == CartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);

            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int CartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == CartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

       /* private double GetPriceBaseOnQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 10)
            {
                return price;
            }
            else
            {
                if (quantity <= 20)
                {
                    return price50* 0.9;
                }
                return price50 * 0.7;
            }
        }*/


      
    }
}