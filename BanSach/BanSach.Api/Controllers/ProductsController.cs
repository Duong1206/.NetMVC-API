using BanSach.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BanSach.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get products with search, filter and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string searchTerm = "",
        [FromQuery] int? categoryId = null,
        [FromQuery] int? coverTypeId = null,
        [FromQuery] double? priceMin = null,
        [FromQuery] double? priceMax = null,
        [FromQuery] int? minRating = null,
        [FromQuery] string sortBy = "name",
        [FromQuery] bool sortAscending = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            CoverTypeId = coverTypeId,
            PriceMin = priceMin,
            PriceMax = priceMax,
            MinRating = minRating,
            SortBy = sortBy,
            SortAscending = sortAscending,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Get product details by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductById(
        [FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == "Product not found"
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }
}
