
using BizFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizFlow.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(company => company.Id);

            entity.Property(company => company.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(company => company.Email)
                .IsRequired()
                .HasMaxLength(320);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(customer => customer.Id);

            entity.Property(customer => customer.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(customer => customer.Email)
                .IsRequired()
                .HasMaxLength(320);

            entity.Property(customer => customer.Phone)
                .HasMaxLength(50);
            entity.OwnsOne(customer => customer.BillingAddress, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("BillingStreet")
                    .IsRequired()
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("BillingCity")
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.PostalCode)
                    .HasColumnName("BillingPostalCode")
                    .IsRequired()
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasColumnName("BillingCountry")
                    .IsRequired()
                    .HasMaxLength(100);
            });
            entity.HasOne<Company>()
                .WithMany()
                .HasForeignKey(customer => customer.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
