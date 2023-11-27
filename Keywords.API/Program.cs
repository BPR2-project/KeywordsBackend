using indexer_api;
using Keywords.Data;
using Keywords.Data.Repositories;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Mappers;
using Keywords.Services;
using Keywords.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using textToSpeech_api;

var builder = WebApplication.CreateBuilder(args);

// Key Vault
// var keyVaultUrl = new Uri(builder.Configuration["KeyVault:Url"]);
// var azureCredential = new DefaultAzureCredential();
// builder.Configuration.AddAzureKeyVault(keyVaultUrl, azureCredential);

// External services
builder.Services.AddScoped<IIndexerClient>(_ => new IndexerClient(builder.Configuration["Indexer:BaseUrl"]));
builder.Services.AddScoped<IAzureTextToSpeechClient>(_ => new AzureTextToSpeechClient(builder.Configuration["TextToSpeech:Url"]));

// var dbConnectionString = builder.Configuration.GetSection(KeyVault.VaultSecrets.keywordsdb.ToString()).Value;
var dbConnectionString = builder.Configuration.GetConnectionString("keywordsdb");

// Add services to the container.
// Db Context
builder.Services.AddDbContext<KeywordsContext>(opt => opt.UseSqlServer(dbConnectionString));

// Repositories
builder.Services.AddScoped<IKeywordEntityRepository, KeywordEntityRepository>();

// Services
builder.Services.AddScoped<IKeywordService, KeywordService>();
builder.Services.AddScoped<IIndexerService, IndexerService>();
builder.Services.AddScoped<IAzureTextToSpeechService, AzureTextToSpeechService>();

// Mapper
builder.Services.AddAutoMapper(typeof(BaseProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();