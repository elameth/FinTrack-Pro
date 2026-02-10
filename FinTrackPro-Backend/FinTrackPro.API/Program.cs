using FinTrackPro.Domain.Entities;
using FinTrackPro.Infrastructure;
using FinTrackPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapPost("/test/users", async (ApplicationDbContext db) =>
{
    var user = User.Create(
        "test@example.com",
        "this-is-a-fake-hash-that-is-long-enough-32chars",
        "John",
        "Doe");

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok(new { user.Id, user.Email, user.FullName, user.CreatedAt });
});

app.MapGet("/test/users", async (ApplicationDbContext db) =>
{
    var users = await db.Users
        .Select(u => new { u.Id, u.Email, u.FullName, u.CreatedAt })
        .ToListAsync();

    return Results.Ok(users);
});

app.Run();