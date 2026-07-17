using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Server.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", async (IRepository<Customer> repo, string? search) =>
        {
            if (string.IsNullOrWhiteSpace(search))
                return Results.Ok(await repo.GetAllAsync());

            var customers = await repo.FindAsync(c =>
                (c.FirstName != null && c.FirstName.Contains(search)) ||
                (c.LastName != null && c.LastName.Contains(search)) ||
                (c.Phone != null && c.Phone.Contains(search)) ||
                (c.Email != null && c.Email.Contains(search)));
            return Results.Ok(customers);
        });

        group.MapGet("/{id}", async (Guid id, IRepository<Customer> repo) =>
        {
            var customer = await repo.GetByIdAsync(id);
            return customer is null ? Results.NotFound() : Results.Ok(customer);
        });

        group.MapGet("/phone/{phone}", async (string phone, IRepository<Customer> repo) =>
        {
            var customers = await repo.FindAsync(c => c.Phone == phone);
            var customer = customers.FirstOrDefault();
            return customer is null ? Results.NotFound() : Results.Ok(customer);
        });

        group.MapPost("/", async (Customer customer, IRepository<Customer> repo) =>
        {
            customer.Id = Guid.NewGuid();
            customer.CreatedAt = DateTime.UtcNow;
            var created = await repo.AddAsync(customer);
            return Results.Created($"/api/customers/{created.Id}", created);
        });

        group.MapPut("/{id}", async (Guid id, Customer customer, IRepository<Customer> repo) =>
        {
            customer.Id = id;
            customer.UpdatedAt = DateTime.UtcNow;
            await repo.UpdateAsync(customer);
            return Results.Ok(customer);
        });

        group.MapGet("/{id}/purchases", async (Guid id, ITransactionRepository txRepo, int page = 1, int pageSize = 20) =>
        {
            var transactions = await txRepo.FindAsync(t =>
                t.CustomerId == id && t.Status == Core.Enums.TransactionStatus.Completed);

            var ordered = transactions.OrderByDescending(t => t.CreatedAt);
            var total = ordered.Count();
            var items = ordered.Skip((page - 1) * pageSize).Take(pageSize);

            return Results.Ok(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Items = items
            });
        });
    }
}
