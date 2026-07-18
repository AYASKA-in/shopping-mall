using System.Text.Json.Serialization;
using ShoppingMall.Data;
using ShoppingMall.Data.DbContext;
using ShoppingMall.Data.Seed;
using ShoppingMall.Business;
using ShoppingMall.Business.Services;
using ShoppingMall.Server.Endpoints;
using ShoppingMall.Server.Middleware;
using ShoppingMall.Server.BackgroundServices;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "ShoppingMallServer";
});

builder.Services.AddDataLayer(builder.Configuration);
builder.Services.AddBusinessLayer();
builder.Services.AddHostedService<CloudSyncService>();
builder.Services.AddHostedService<SessionCleanupService>();
builder.Services.AddScoped<CloudBackupService>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Shopping Mall API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShoppingMallDbContext>();
    await db.Database.MigrateAsync();
    await DbInitializer.InitializeAsync(db);
}

app.UseCors();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SessionAuthMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAuthEndpoints();
app.MapPosEndpoints();
app.MapProductEndpoints();
app.MapInventoryEndpoints();
app.MapProcurementEndpoints();
app.MapReportEndpoints();
app.MapSyncEndpoints();
app.MapAdminEndpoints();
app.MapCustomerEndpoints();
app.MapPromotionEndpoints();
app.MapLoyaltyEndpoints();
app.MapTransferEndpoints();
app.MapUpiEndpoints();

app.Run();
