namespace ShoppingMall.Server.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (IProductRepository repo, string? search) =>
        {
            if (!string.IsNullOrWhiteSpace(search))
                return Results.Ok(await repo.SearchAsync(search));
            return Results.Ok(await repo.GetAllAsync());
        });

        group.MapGet("/{id}", async (Guid id, IProductRepository repo) =>
        {
            var product = await repo.GetByIdAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        group.MapGet("/barcode/{code}", async (string code, IProductRepository repo) =>
        {
            var product = await repo.GetByBarcodeAsync(code);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        group.MapPost("/", async (Product product, IProductRepository repo) =>
        {
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;
            var created = await repo.AddAsync(product);
            return Results.Created($"/api/products/{created.Id}", created);
        });

        group.MapPut("/{id}", async (Guid id, Product product, IProductRepository repo) =>
        {
            product.Id = id;
            product.UpdatedAt = DateTime.UtcNow;
            await repo.UpdateAsync(product);
            return Results.Ok(product);
        });

        group.MapGet("/categories", async (IRepository<Category> repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapPost("/categories", async (Category category, IRepository<Category> repo) =>
        {
            category.Id = Guid.NewGuid();
            var created = await repo.AddAsync(category);
            return Results.Created($"/api/products/categories/{created.Id}", created);
        });
    }
}
