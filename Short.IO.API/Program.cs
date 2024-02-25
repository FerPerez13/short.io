using Azure.Data.Tables;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Short.IO.API;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var storageConfiguration = new StorageConfiguration
{
    ConnectionString = builder.Configuration.GetValue<string>("AzureTableStorage:ConnectionString"),
    AzureTable = new AzureTableConfiguration
    {
        UrlTableName = builder.Configuration.GetValue<string>("AzureTableStorage:UrlTableName"),
        PartitionKey = builder.Configuration.GetValue<string>("AzureTableStorage:PartitionKey"),
        RowKey = builder.Configuration.GetValue<string>("AzureTableStorage:RowKey"),
    }
};

builder.Services.AddSingleton(storageConfiguration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// #region Endpoints
app.MapGet("/{shortUrl}", async (string shortUrl) =>
{
    var tableServiceClient = new TableServiceClient(storageConfiguration.ConnectionString);
    var tableClient = tableServiceClient.GetTableClient(storageConfiguration.AzureTable.UrlTableName);
    tableClient.CreateIfNotExists();

    var urlRedirect = tableClient.Query<UrlRedirect>().FirstOrDefault(u => u.ShortUrl == shortUrl);
    if (urlRedirect == null)
    {
        return Results.NotFound();
    }
    urlRedirect.Clicks++;
    tableClient.UpsertEntity(urlRedirect);

    return Results.Redirect(urlRedirect.LongUrl);
});

// Crear registro para una url corta
app.MapPost("/create", async (string longUrl) =>
{
    var tableServiceClient = new TableServiceClient(storageConfiguration.ConnectionString);
    var tableClient = tableServiceClient.GetTableClient(storageConfiguration.AzureTable.UrlTableName);
    tableClient.CreateIfNotExists();
    
    var urlRedirectExists = tableClient.Query<UrlRedirect>().FirstOrDefault(u => u.LongUrl == longUrl);
    if (urlRedirectExists != null)
    {
        return new OkObjectResult(urlRedirectExists);
    }

    var urlRedirect = new UrlRedirect
    {
        PartitionKey = Guid.NewGuid().ToString(),
        RowKey = storageConfiguration.AzureTable.RowKey,
        ShortUrl = Guid.NewGuid().ToString().Substring(0, 5),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        LongUrl = longUrl,
    };
    
    tableClient.AddEntity(urlRedirect);
    
    return new OkObjectResult(urlRedirect.ShortUrl);
});

// Listar todos los registros de la tabla
app.MapGet("/list", async () =>
{
    var tableServiceClient = new TableServiceClient(storageConfiguration.ConnectionString);
    var tableClient = tableServiceClient.GetTableClient(storageConfiguration.AzureTable.UrlTableName);
    tableClient.CreateIfNotExists();
    
    var urlRedirects = tableClient.Query<UrlRedirect>().ToList();
    
    return new OkObjectResult(urlRedirects);
});

// Eliminar un registro de la tabla por su shortUrl
app.MapDelete("/delete/{shortUrl}", async (string shortUrl) =>
{
    var tableServiceClient = new TableServiceClient(storageConfiguration.ConnectionString);
    var tableClient = tableServiceClient.GetTableClient(storageConfiguration.AzureTable.UrlTableName);
    tableClient.CreateIfNotExists();
    
    var urlRedirect = tableClient.Query<UrlRedirect>().FirstOrDefault(u => u.ShortUrl == shortUrl);
    if (urlRedirect == null)
    {
        return Results.NotFound();
    }
    
    tableClient.DeleteEntity(urlRedirect.PartitionKey, urlRedirect.RowKey);
    
    return Results.Ok();
});

// #endregion

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
