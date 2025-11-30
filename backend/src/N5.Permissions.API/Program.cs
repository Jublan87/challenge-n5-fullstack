using Microsoft.EntityFrameworkCore;
using Nest;
using N5.Permissions.API.Middleware;
using N5.Permissions.Application.Commands;
using N5.Permissions.Application.Mappings;
using N5.Permissions.Application.Queries;
using N5.Permissions.Domain.Interfaces;
using N5.Permissions.Infrastructure.Persistence;
using N5.Permissions.Infrastructure.Repositories;
using N5.Permissions.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Handlers
builder.Services.AddScoped<RequestPermissionHandler>();
builder.Services.AddScoped<ModifyPermissionHandler>();
builder.Services.AddScoped<GetPermissionsHandler>();

// Elasticsearch
var elasticsearchUrl = builder.Configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
var elasticSettings = new ConnectionSettings(new Uri(elasticsearchUrl))
    .DefaultIndex("permissions");
builder.Services.AddSingleton<IElasticClient>(new ElasticClient(elasticSettings));
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

// Kafka
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    if (context.Database.IsRelational())
    {
        context.Database.Migrate();
    }
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }