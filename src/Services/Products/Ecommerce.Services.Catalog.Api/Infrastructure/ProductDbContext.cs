using BuildingBlocks.EfCore.Persistence.Commons;
using Ecommerce.Services.Catalog.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Catalog.Api.Infrastructure;

public class ProductDbContext : EfDbContextBase
{
    public DbSet<Product> Products { get; set; }

    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
         
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}