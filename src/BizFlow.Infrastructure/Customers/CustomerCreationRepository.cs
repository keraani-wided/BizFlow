using BizFlow.Application.Customers.Interfaces;
using BizFlow.Domain.Entities;
using BizFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BizFlow.Infrastructure.Customers;

public sealed class CustomerCreationRepository
    : ICustomerCreationRepository
{
    private readonly AppDbContext _db;

    public CustomerCreationRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> CompanyExistsAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        return _db.Companies.AnyAsync(
            company => company.Id == companyId,
            cancellationToken);
    }

    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken)
    {
        _db.Customers.Add(customer);

        await _db.SaveChangesAsync(cancellationToken);
    }
}