using BizFlow.Application.Customers.Interfaces;
using BizFlow.Domain.Common;
using BizFlow.Domain.Entities;
using MediatR;

namespace BizFlow.Application.Customers.Commands;

public sealed class CreateCustomerCommandHandler
    : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly ICustomerCreationRepository _repository;

    public CreateCustomerCommandHandler(
        ICustomerCreationRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateCustomerResult> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        bool companyExists = await _repository.CompanyExistsAsync(
            request.CompanyId,
            cancellationToken);

        if (!companyExists)
        {
            throw new ArgumentException(
                "L'entreprise spécifiée n'existe pas.",
                nameof(request.CompanyId));
        }

        var address = new Address(
            request.Street,
            request.City,
            request.PostalCode,
            request.Country);

        var customer = Customer.Create(
            request.CompanyId,
            request.Name,
            request.Email,
            address,
            request.Phone);

        await _repository.AddAsync(customer, cancellationToken);

        return new CreateCustomerResult(
            customer.Id,
            customer.CompanyId,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.BillingAddress.Street,
            customer.BillingAddress.City,
            customer.BillingAddress.PostalCode,
            customer.BillingAddress.Country,
            customer.IsActive,
            customer.CreatedAtUtc);
    }
}