
namespace BizFlow.Domain.Entities;

public sealed class Company
{
    /// <summary>
    /// Id : identifie l'entreprise.

    // private set : empêche les autres classes de modifier directement les propriétés.

    //private Company() : permet notamment à EF Core de matérialiser l'entité plus tard.

    //Company.Create() : centralise la création et vérifie les données obligatoires.

    //CreatedAtUtc : enregistre la date de création en UTC.
    /// </summary>
    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    private Company() { }

    public static Company Create(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Le nom de l'entreprise est obligatoire.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException(
                "Adresse email invalide.",
                nameof(email));

        return new Company
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}