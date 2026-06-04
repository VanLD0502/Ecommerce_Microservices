using Ecommerce.Services.Catalog.Api.Models.Dtos;
using Ecommerce.Services.Catalog.Api.Models.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Services.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController:  ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var result = await _productService.GetProductsAsync();

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var result = await _productService.GetProductByIdAsync(id);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductRequest request)
    {
        var result = await _productService.AddProductAsync(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, ProductRequest request)
    {
        var result = await _productService.UpdateProductAsync(id, request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return StatusCode(result.GetHttpStatusCode(), result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return  StatusCode(result.GetHttpStatusCode(), result.Message);
    }
}