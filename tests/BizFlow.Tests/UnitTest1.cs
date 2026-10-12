
using BizFlow.Domain.Common;
using BizFlow.Domain.Entities;

namespace BizFlow.Tests;

public class CustomerTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateActiveCustomer()
    {
        // Arrange : pr�parer les donn�es
        var companyId = Guid.NewGuid();

        var address = new Address(
            "Rue de Tunis",
            "Tunis",
            "00216",
            "Tunisie");

        // Act : cr�er le client
        var customer = Customer.Create(
            companyId,
            "Client Test",
            "CLIENT@EXAMPLE.COM",
            address,
            "+21612345678");

        // Assert : v�rifier le r�sultat
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal(companyId, customer.CompanyId);
        Assert.Equal("Client Test", customer.Name);
        Assert.Equal("client@example.com", customer.Email);
        Assert.Equal("+21612345678", customer.Phone);
        Assert.True(customer.IsActive);
        Assert.NotEqual(default(DateTime), customer.CreatedAtUtc);
        Assert.Null(customer.UpdatedAtUtc);
        Assert.Equal("Tunis", customer.BillingAddress.City);
    }


    [Fact]
    public void Create_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        var address = new Address(
            "Rue de Tunis",
            "Tunis",
            "00216",
            "Tunisie");

        // Act et Assert
        Assert.Throws<ArgumentException>(() =>
            Customer.Create(
                companyId,
                "Client Test",
                "email-invalide",
                address));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalseAndUpdateTimestamp()
    {
        // Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            "Client Test",
            "client@example.com",
            new Address(
                "Rue de Tunis",
                "Tunis",
                "00216",
                "Tunisie"));

        // Act
        customer.Deactivate();

        // Assert
        Assert.False(customer.IsActive);
        Assert.NotNull(customer.UpdatedAtUtc);
    }


    [Fact]
    public void Activate_ShouldSetIsActiveToTrueAndUpdateTimestamp()
    {
        // Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            "Client Test",
            "client@example.com",
            new Address(
                "Rue de Tunis",
                "Tunis",
                "00216",
                "Tunisie"));

        customer.Deactivate();

        // Act
        customer.Activate();

        // Assert
        Assert.True(customer.IsActive);
        Assert.NotNull(customer.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateContactInfo_WithValidData_ShouldUpdateCustomer()
    {
        // Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            "Ancien Nom",
            "ancien@example.com",
            new Address(
                "Rue de Tunis",
                "Tunis",
                "00216",
                "Tunisie"));

        // Act
        customer.UpdateContactInfo(
            "Nouveau Nom",
            "NOUVEAU@EXAMPLE.COM",
            "+21698765432");

        // Assert
        Assert.Equal("Nouveau Nom", customer.Name);
        Assert.Equal("nouveau@example.com", customer.Email);
        Assert.Equal("+21698765432", customer.Phone);
        Assert.NotNull(customer.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateBillingAddress_ShouldUpdateAddressAndTimestamp()
    {
        // Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            "Client Test",
            "client@example.com",
            new Address(
                "Ancienne Rue",
                "Tunis",
                "00216",
                "Tunisie"));

        var newAddress = new Address(
            "Nouvelle Rue",
            "Sfax",
            "00316",
            "Tunisie");

        // Act
        customer.UpdateBillingAddress(newAddress);

        // Assert
        Assert.Equal("Nouvelle Rue", customer.BillingAddress.Street);
        Assert.Equal("Sfax", customer.BillingAddress.City);
        Assert.Equal("00316", customer.BillingAddress.PostalCode);
        Assert.NotNull(customer.UpdatedAtUtc);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var address = new Address(
            "Rue de Tunis",
            "Tunis",
            "00216",
            "Tunisie");

        // Act et Assert
        Assert.Throws<ArgumentException>(() =>
            Customer.Create(
                Guid.NewGuid(),
                " ",
                "client@example.com",
                address));
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ShouldThrowArgumentException()
    {
        // Arrange
        var address = new Address(
            "Rue de Tunis",
            "Tunis",
            "00216",
            "Tunisie");

        // Act et Assert
        Assert.Throws<ArgumentException>(() =>
            Customer.Create(
                Guid.Empty,
                "Client Test",
                "client@example.com",
                address));
    }

    [Fact]
    public void UpdateContactInfo_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            "Client Test",
            "client@example.com",
            new Address(
                "Rue de Tunis",
                "Tunis",
                "00216",
                "Tunisie"));

        // Act et Assert
        Assert.Throws<ArgumentException>(() =>
            customer.UpdateContactInfo(
                "Nouveau Nom",
                "email-invalide",
                "+21698765432"));
    }

    [Fact]
    public void UpdateContactInfo_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            "Client Test",
            "client@example.com",
            new Address(
                "Rue de Tunis",
                "Tunis",
                "00216",
                "Tunisie"));

        // Act et Assert
        Assert.Throws<ArgumentException>(() =>
            customer.UpdateContactInfo(
                " ",
                "nouveau@example.com",
                "+21698765432"));
    }

    [Fact]
    public void CompanyCreate_WithValidData_ShouldCreateCompany()
    {
        // Arrange
        var name = "Entreprise Test";
        var email = "ENTREPRISE@EXAMPLE.COM";

        // Act
        var company = Company.Create(name, email);

        // Assert
        Assert.NotEqual(Guid.Empty, company.Id);
        Assert.Equal("Entreprise Test", company.Name);
        Assert.Equal("entreprise@example.com", company.Email);
        Assert.NotEqual(default(DateTime), company.CreatedAtUtc);
    }

    [Fact]
    public void CompanyCreate_WithEmptyName_ShouldThrowArgumentException()
    {
        // Act et Assert
        Assert.Throws<ArgumentException>(() =>
            Company.Create(
                " ",
                "entreprise@example.com"));
    }

    [Fact]
    public void CompanyCreate_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Act et Assert
        Assert.Throws<ArgumentException>(() =>
            Company.Create(
                "Entreprise Test",
                "email-invalide"));
    }












}
