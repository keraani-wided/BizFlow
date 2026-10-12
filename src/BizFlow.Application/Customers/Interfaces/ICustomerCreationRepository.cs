using BizFlow.Domain.Entities;

namespace BizFlow.Application.Customers.Interfaces;

public interface ICustomerCreationRepository
{
    Task<bool> CompanyExistsAsync(
        Guid companyId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken);
}