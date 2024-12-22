
using Amazon.DynamoDBv2.Model;
using BanSach.DataAcess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var bestSeller = await _context.Products
                .OrderByDescending(p => p.SoldCount)
                .FirstOrDefaultAsync();

            if (bestSeller != null)
            {
                var response = new DialogflowResponse
                {
                    FulfillmentText = $"The best seller is {bestSeller.Name} with {bestSeller.SoldCount} units sold."
                };
                return Ok(response);
            }

            return Ok(new DialogflowResponse { FulfillmentText = "Sorry, I couldn't find the best seller at the moment." });
        }
    }

    public class DialogflowRequest
    {
        public QueryResult queryResult { get; set; }
    }

    public class DialogflowResponse
    {
        public string FulfillmentText { get; set; }
    }

}
