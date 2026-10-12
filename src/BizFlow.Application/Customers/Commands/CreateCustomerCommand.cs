using MediatR;

namespace BizFlow.Application.Customers.Commands;

public sealed record CreateCustomerCommand(
    Guid CompanyId,
    string Name,
    string Email,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string? Phone
) : IRequest<CreateCustomerResult>;

public sealed record CreateCustomerResult(
    Guid Id,
    Guid CompanyId,
    string Name,
    string Email,
    string? Phone,
    string Street,
    string City,
    string PostalCode,
    string Country,
    bool IsActive,
    DateTime CreatedAtUtc
);