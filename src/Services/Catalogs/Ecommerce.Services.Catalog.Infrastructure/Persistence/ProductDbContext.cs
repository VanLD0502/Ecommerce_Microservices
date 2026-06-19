using BuildingBlocks.EfCore.Persistence.Commons;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Ecommerce.Services.Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Catalog.Infrastructure.Persistence;

public class ProductDbContext(DbContextOptions<ProductDbContext> options, IInMemoryBus bus) : EfDbContextBase(options, bus)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductOption> ProductOptions { get; set; }
    public DbSet<ProductOptionValue> ProductOptionValues { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductVariantOption> ProductVariantOptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Status)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            entity.HasMany(p => p.Options)
                  .WithOne()
                  .HasForeignKey(c => c.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(p => p.Options)
                  .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.HasMany(p => p.Variants)
                  .WithOne()
                  .HasForeignKey(v => v.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(p => p.Variants)
                  .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<ProductOption>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Name).HasMaxLength(255);

            entity.HasMany(o => o.Values)
                  .WithOne()
                  .HasForeignKey(v => v.OptionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(o => o.Values)
                  .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<ProductOptionValue>(entity =>
        {
            entity.HasKey(ov => ov.Id);
            entity.Property(ov => ov.Value).HasMaxLength(255);
            entity.Property(ov => ov.ImageUrl).HasMaxLength(1000);
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Price).HasColumnType("decimal(18,2)");
            entity.Property(v => v.Sku).HasMaxLength(50);

            entity.HasQueryFilter(v => !v.IsDeleted);

            entity.HasMany(v => v.Options)
                  .WithOne(vo => vo.Variant)
                  .HasForeignKey(vo => vo.VariantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(v => v.Options)
                  .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<ProductVariantOption>(entity =>
        {
            entity.HasKey(vo => new { vo.VariantId, vo.OptionValueId });

            entity.HasOne(vo => vo.OptionValue)
                  .WithMany()
                  .HasForeignKey(vo => vo.OptionValueId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
