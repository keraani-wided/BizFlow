using BizFlow.Domain.Common;

namespace BizFlow.Domain.Entities;

public sealed class Customer
{
    // Identifiant unique de l'entité
    public Guid Id { get; private set; }

    // Multi-tenant : appartient obligatoirement à une entreprise
    public Guid CompanyId { get; private set; }

    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Phone { get; private set; }

    // Value Object pour l'adresse
    public Address BillingAddress { get; private set; } = null!;

    // Statut d'activation
    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // Constructeur privé requis pour EF Core
    private Customer() { }

    // Usine Métier (Factory Method)
    public static Customer Create(
        Guid companyId,
        string name,
        string email,
        Address billingAddress,
        string? phone = null)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("L'entreprise est obligatoire.", nameof(companyId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom du client est obligatoire.", nameof(name));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Adresse email invalide.", nameof(email));

        ArgumentNullException.ThrowIfNull(billingAddress);

        return new Customer
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            BillingAddress = billingAddress,
            Phone = phone?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    // Méthodes Métier

    public void UpdateContactInfo(string name, string email, string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom du client est obligatoire.", nameof(name));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Adresse email invalide.", nameof(email));

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateBillingAddress(Address newAddress)
    {
        ArgumentNullException.ThrowIfNull(newAddress);
        BillingAddress = newAddress;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}