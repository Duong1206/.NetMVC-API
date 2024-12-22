using BanSach.DataAcess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BanSachApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DialogflowController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DialogflowController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Webhook")]
        public async Task<IActionResult> Webhook([FromBody] DialogflowRequest request)
        {
            var intentName = request?.QueryResult?.Intent?.DisplayName;

            switch (intentName)
            {
                case "BestSeller":
                    return await HandleBestSeller();

                case "CheckProduct":
                    return await HandleCheckProduct(request);

                default:
                    return Ok(new DialogflowResponse
                    {
                        FulfillmentText = "Sorry, I couldn't handle that request."
                    });
            }
        }

        private async Task<IActionResult> HandleBestSeller()
        {
            var bestSeller = await _context.Products
                .OrderByDescending(p => p.SoldCount)
                .FirstOrDefaultAsync();

            if (bestSeller != null)
            {
                return Ok(new DialogflowResponse
                {
                    FulfillmentText = $"The best seller is {bestSeller.Name} with {bestSeller.SoldCount} units sold."
                });
            }

            return Ok(new DialogflowResponse
            {
                FulfillmentText = "Sorry, I couldn't find the best seller at the moment."
            });
        }

        private async Task<IActionResult> HandleCheckProduct(DialogflowRequest request)
        {
            var productName = request?.QueryResult?.Parameters?["productName"]?.ToString().ToLower();

            if (string.IsNullOrEmpty(productName))
            {
                return Ok(new DialogflowResponse
                {
                    FulfillmentText = "I didn't catch the product name. Could you repeat it?"
                });
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.Name, $"%{productName}%"));

            if (product != null)
            {
                if (product.Quantity < 1)
                {
                    return Ok(new DialogflowResponse
                    {
                        FulfillmentText = $"The product {product.Name} is currently out of stock."
                    });
                }

                return Ok(new DialogflowResponse
                {
                    FulfillmentText = $"Yes, we have {product.Name} in stock with {product.Quantity} units available."
                });
            }

            return Ok(new DialogflowResponse
            {
                FulfillmentText = $"Sorry, we currently don't have a product named {productName}."
            });
        }
    }

    public class DialogflowRequest
    {
        public QueryResult QueryResult { get; set; }
    }

    public class QueryResult
    {
        public Intent Intent { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    public class Intent
    {
        public string DisplayName { get; set; }
    }

    public class DialogflowResponse
    {
        public string FulfillmentText { get; set; }
    }
}
