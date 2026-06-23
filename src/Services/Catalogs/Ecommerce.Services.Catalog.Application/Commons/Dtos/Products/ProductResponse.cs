using System;
using System.Collections.Generic;

namespace Ecommerce.Services.Catalog.Application.Commons.Dtos.Products;

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public List<ProductOptionDto> Options { get; set; } = new();
    public List<ProductVariantDto> Variants { get; set; } = new();
}

public class ProductOptionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int SortOrder { get; set; }
    public List<ProductOptionValueDto> Values { get; set; } = new();
}

public class ProductOptionValueDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
}

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public int AvailableStocks { get; set; }
    public int ReservedStocks { get; set; }
    public List<ProductVariantOptionDto> Options { get; set; } = new();
}

public class ProductVariantOptionDto
{
    public Guid OptionValueId {get; set;}
}