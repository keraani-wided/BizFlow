using BizFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BizFlow.Domain.Common;
using BizFlow.Domain.Entities;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "La chaîne de connexion 'DefaultConnection' est introuvable.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/api/companies", async (AppDbContext db) =>
{
    var companies = await db.Companies
        .AsNoTracking()
        .OrderBy(company => company.Name)
        .Select(company => new
        {
            company.Id,
            company.Name,
            company.Email,
            company.CreatedAtUtc
        })
        .ToListAsync();

    return Results.Ok(companies);
})
.WithName("GetCompanies")
.WithOpenApi();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};




app.MapPost("/api/companies", async (CreateCompanyRequest request, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) ||
        string.IsNullOrWhiteSpace(request.Email))
    {
        return Results.BadRequest(new
        {
            message = "Le nom et l'adresse e-mail sont obligatoires."
        });
    }

    BizFlow.Domain.Entities.Company company;

    try
    {
        company = BizFlow.Domain.Entities.Company.Create(
            request.Name,
            request.Email);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new
        {
            message = ex.Message
        });
    }

    db.Companies.Add(company);
    await db.SaveChangesAsync();

    return Results.Created($"/api/companies/{company.Id}", new
    {
        company.Id,
        company.Name,
        company.Email,
        company.CreatedAtUtc
    });
})
.WithName("CreateCompany")
.WithOpenApi();


app.MapPost("/api/customers", async (CreateCustomerRequest request, AppDbContext db) =>
{
    var companyExists = await db.Companies
        .AnyAsync(company => company.Id == request.CompanyId);

    if (!companyExists)
    {
        return Results.BadRequest(new
        {
            message = "L'entreprise spécifiée n'existe pas."
        });
    }

    Customer customer;

    try
    {
        var address = new Address(
            request.Street,
            request.City,
            request.PostalCode,
            request.Country);

        customer = Customer.Create(
            request.CompanyId,
            request.Name,
            request.Email,
            address,
            request.Phone);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new
        {
            message = ex.Message
        });
    }

    db.Customers.Add(customer);
    await db.SaveChangesAsync();

    return Results.Created($"/api/customers/{customer.Id}", new
    {
        customer.Id,
        customer.CompanyId,
        customer.Name,
        customer.Email,
        customer.Phone,
        BillingAddress = new
        {
            customer.BillingAddress.Street,
            customer.BillingAddress.City,
            customer.BillingAddress.PostalCode,
            customer.BillingAddress.Country
        },
        customer.IsActive,
        customer.CreatedAtUtc
    });
})
.WithName("CreateCustomer")
.WithOpenApi();

app.MapGet("/api/customers", async (Guid companyId, AppDbContext db) =>
{
    if (companyId == Guid.Empty)
    {
        return Results.BadRequest(new
        {
            message = "L'identifiant de l'entreprise est obligatoire."
        });
    }

    var customers = await db.Customers
        .AsNoTracking()
        .Where(customer => customer.CompanyId == companyId)
        .OrderBy(customer => customer.Name)
        .Select(customer => new
        {
            customer.Id,
            customer.CompanyId,
            customer.Name,
            customer.Email,
            customer.Phone,
            BillingAddress = new
            {
                customer.BillingAddress.Street,
                customer.BillingAddress.City,
                customer.BillingAddress.PostalCode,
                customer.BillingAddress.Country
            },
            customer.IsActive,
            customer.CreatedAtUtc,
            customer.UpdatedAtUtc
        })
        .ToListAsync();

    return Results.Ok(customers);
})
.WithName("GetCustomers")
.WithOpenApi();

app.MapGet("/api/customers/{id:guid}", async (Guid id, AppDbContext db) =>
{
    var customer = await db.Customers
        .AsNoTracking()
        .Where(customer => customer.Id == id)
        .Select(customer => new
        {
            customer.Id,
            customer.CompanyId,
            customer.Name,
            customer.Email,
            customer.Phone,
            BillingAddress = new
            {
                customer.BillingAddress.Street,
                customer.BillingAddress.City,
                customer.BillingAddress.PostalCode,
                customer.BillingAddress.Country
            },
            customer.IsActive,
            customer.CreatedAtUtc,
            customer.UpdatedAtUtc
        })
        .FirstOrDefaultAsync();

    if (customer is null)
    {
        return Results.NotFound(new
        {
            message = "Client introuvable."
        });
    }

    return Results.Ok(customer);
})
.WithName("GetCustomerById")
.WithOpenApi();

app.MapPut("/api/customers/{id:guid}", async (Guid id, UpdateCustomerRequest request, AppDbContext db) =>
{
    var customer = await db.Customers
        .FirstOrDefaultAsync(customer => customer.Id == id);

    if (customer is null)
    {
        return Results.NotFound(new
        {
            message = "Client introuvable."
        });
    }

    try
    {
        var address = new Address(
            request.Street,
            request.City,
            request.PostalCode,
            request.Country);

        customer.UpdateContactInfo(
            request.Name,
            request.Email,
            request.Phone);

        customer.UpdateBillingAddress(address);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new
        {
            message = ex.Message
        });
    }

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        customer.Id,
        customer.CompanyId,
        customer.Name,
        customer.Email,
        customer.Phone,
        BillingAddress = new
        {
            customer.BillingAddress.Street,
            customer.BillingAddress.City,
            customer.BillingAddress.PostalCode,
            customer.BillingAddress.Country
        },
        customer.IsActive,
        customer.CreatedAtUtc,
        customer.UpdatedAtUtc
    });
})
.WithName("UpdateCustomer")
.WithOpenApi();

app.MapPatch("/api/customers/{id:guid}/deactivate", async (Guid id, AppDbContext db) =>
{
    var customer = await db.Customers
        .FirstOrDefaultAsync(customer => customer.Id == id);

    if (customer is null)
    {
        return Results.NotFound(new
        {
            message = "Client introuvable."
        });
    }

    customer.Deactivate();

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        customer.Id,
        customer.CompanyId,
        customer.Name,
        customer.IsActive,
        customer.UpdatedAtUtc
    });
})
.WithName("DeactivateCustomer")
.WithOpenApi();

app.MapPatch("/api/customers/{id:guid}/activate", async (Guid id, AppDbContext db) =>
{
    var customer = await db.Customers
        .FirstOrDefaultAsync(customer => customer.Id == id);

    if (customer is null)
    {
        return Results.NotFound(new
        {
            message = "Client introuvable."
        });
    }

    customer.Activate();

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        customer.Id,
        customer.CompanyId,
        customer.Name,
        customer.IsActive,
        customer.UpdatedAtUtc
    });
})
.WithName("ActivateCustomer")
.WithOpenApi();


app.Run();
record CreateCompanyRequest(string Name, string Email);
record CreateCustomerRequest(
    Guid CompanyId,
    string Name,
    string Email,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string? Phone);
record UpdateCustomerRequest(
    string Name,
    string Email,
    string? Phone,
    string Street,
    string City,
    string PostalCode,
    string Country);
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
