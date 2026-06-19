using Ecommerce.Services.Catalog.Api.Models.Dtos;
using Ecommerce.Services.Catalog.Application.Features.Products.Commands.CreateProduct;
using Ecommerce.Services.Catalog.Application.Features.Products.Commands.DeleteProduct;
using Ecommerce.Services.Catalog.Application.Features.Products.Commands.UpdateProduct;
using Ecommerce.Services.Catalog.Application.Features.Products.Commands.SetupProductVariants;
using Ecommerce.Services.Catalog.Application.Features.Products.Queries.GetProductById;
using Ecommerce.Services.Catalog.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Ecommerce.Services.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var result = await sender.Send(new GetProductsQuery());

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var result = await sender.Send(new GetProductByIdQuery(id));

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductRequest request)
    {
        var result = await sender.Send(new CreateProductCommand(
            request.Name,
            request.Description
        ));

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, ProductRequest request)
    {
        var result = await sender.Send(new UpdateProductCommand(
            id,
            request.Name,
            request.Description
        ));

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }

    [HttpPut("{id}/init-variants")]
    public async Task<IActionResult> UpdateProductVariantsSetup(Guid id, [FromBody] SetupProductVariantsCommand command)
    {
        var result = await sender.Send(command with { ProductId = id });

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await sender.Send(new DeleteProductCommand(id));

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }
}