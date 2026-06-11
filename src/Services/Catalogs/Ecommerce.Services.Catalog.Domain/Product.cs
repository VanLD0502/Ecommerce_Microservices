using BuildingBlocks.Shared.Domains;
using Ecommerce.Services.Catalog.Domain.Rules;

namespace Ecommerce.Services.Catalog.Domain;

public class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stocks { get; private set; }

    private Product() { }

    public Product(string name, string description, decimal price, int stocks)
    {
        Check(new ProductNameCannotBeEmptyRule(name));
        Check(new ProductPriceMustBePositiveRule(price));
        Check(new ProductStocksCannotBeNegativeRule(stocks));

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        Stocks = stocks;
    }

    public void UpdateDetails(string name, string description)
    {
        Check(new ProductNameCannotBeEmptyRule(name));
        Name = name;
        Description = description;
    }

    public void UpdatePrice(decimal price)
    {
        Check(new ProductPriceMustBePositiveRule(price));
        Price = price;
    }

    public void UpdateStocks(int stocks)
    {
        Check(new ProductStocksCannotBeNegativeRule(stocks));
        Stocks = stocks;
    }
}
