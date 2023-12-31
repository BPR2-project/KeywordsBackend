using indexer_api;
using Keywords.Data;
using Keywords.Data.Repositories;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Mappers;
using Keywords.Services;
using Keywords.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using speechToText_api;

var builder = WebApplication.CreateBuilder(args);

// External services
builder.Services.AddScoped<IIndexerClient>(_ => new IndexerClient(builder.Configuration["Indexer:BaseUrl"]));
builder.Services.AddScoped<IKeyPhraseClient>(_ => new KeyPhraseClient(builder.Configuration["KeyPhrase:BaseUrl"]));
builder.Services.AddScoped<IAzureSpeechToTextClient>(_ => new AzureSpeechToTextClient(builder.Configuration["SpeechToText:BaseUrl"]));

var dbConnectionString = builder.Configuration.GetConnectionString("KeywordsDb");

// Add services to the container.
// Db Context
builder.Services.AddDbContext<KeywordsContext>(opt => opt.UseSqlServer(dbConnectionString));

// Repositories
builder.Services.AddScoped<IKeywordEntityRepository, KeywordEntityRepository>();
builder.Services.AddScoped<IIndexerEntityRepository, IndexerEntityRepository>();

// Services
builder.Services.AddScoped<IKeywordService, KeywordService>();
builder.Services.AddScoped<IIndexerService, IndexerService>();
builder.Services.AddScoped<ITextToSpeechService, TextToSpeechService>();
builder.Services.AddScoped<ISpeechToTextService, SpeechToTextService>();

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

public partial class Program { }