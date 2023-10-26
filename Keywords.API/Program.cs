using Keywords.Data;
using Keywords.Data.Repositories;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Mappers;
using Keywords.Services;
using Keywords.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("KeywordsDb");

// Add services to the container.
// Db Context
builder.Services.AddDbContext<KeywordsContext>(opt => opt.UseSqlServer(dbConnectionString));

// Repositories
builder.Services.AddScoped<IKeywordEntityRepository, KeywordEntityRepository>();

// Services
builder.Services.AddScoped<IKeywordService, KeywordService>();

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