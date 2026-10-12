namespace BizFlow.Domain.Common;

public sealed class Address
{
    public string Street { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;
    public string Country { get; private set; } = null!;

    private Address() { }

    public Address(
        string street,
        string city,
        string postalCode,
        string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("La rue est obligatoire.", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("La ville est obligatoire.", nameof(city));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Le code postal est obligatoire.", nameof(postalCode));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Le pays est obligatoire.", nameof(country));

        Street = street.Trim();
        City = city.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
    }
}