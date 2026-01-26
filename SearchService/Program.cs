using Typesense.Setup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

var typesenseUri = builder.Configuration["services:typesense:typesense:0"];

if (string.IsNullOrEmpty(typesenseUri)) throw new InvalidOperationException("typesense not found in config");

var typesenseApiKey = builder.Configuration["typesense-api-key"];
if (string.IsNullOrEmpty(typesenseApiKey))
    throw new InvalidOperationException("Typesense API key not found in configuration");


var uri = new Uri(typesenseUri);

builder.Services.AddTypesenseClient(config =>
{
    config.ApiKey = typesenseApiKey;
    config.Nodes =
    [
        new(uri.Host, uri.Port.ToString(), uri.Scheme)
    ];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//adding this as a trigger test for sonarqube.

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

await app.RunAsync();