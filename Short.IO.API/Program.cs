using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Short.IO.API;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// comprobar si la base de datos de SQLite esta creada y si no crearla
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "shortio.db");
if (!File.Exists(dbPath))
{
    using var db = new ShortIoContext(
        new DbContextOptionsBuilder<ShortIoContext>()
        .UseSqlite($"Data Source={dbPath}")
        .Options);
    db.Database.EnsureCreated();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/{shortUrl}", async (string shortUrl) =>
{
    // usando la base de datos de SQLite para obtener la url larga
    using var db = new ShortIoContext(new DbContextOptionsBuilder<ShortIoContext>().UseSqlite($"Data Source={dbPath}").Options);

    var urlRedirect = await db.UrlRedirects
        .Where(u => u.ShortUrl == shortUrl)
        .FirstOrDefaultAsync();

    urlRedirect.Clicks++;
    await db.SaveChangesAsync();

    db.Dispose();

    return Results.Redirect(urlRedirect.LongUrl);
});

// Crear registro para una url corta
app.MapPost("/", async (UrlRedirect urlRedirect) =>
{
    // usando la base de datos de SQLite para obtener la url larga
    using var db = new ShortIoContext(new DbContextOptionsBuilder<ShortIoContext>().UseSqlite($"Data Source={dbPath}").Options);

    // comprobar si la url larga ya existe
    var urlRedirectExists = await db.UrlRedirects.Where(u => u.LongUrl == urlRedirect.LongUrl).FirstOrDefaultAsync();

    if (urlRedirectExists != null)
    {
        return new OkObjectResult(urlRedirectExists);
    }

    // crear la url corta
    urlRedirect.Id = Guid.NewGuid();
    urlRedirect.ShortUrl = Guid.NewGuid().ToString().Substring(0, 5);
    urlRedirect.CreatedAt = DateTime.Now;
    urlRedirect.UpdatedAt = DateTime.Now;

    await db.UrlRedirects.AddAsync(urlRedirect);
    await db.SaveChangesAsync();

    db.Dispose();

    return new OkObjectResult(urlRedirect);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
