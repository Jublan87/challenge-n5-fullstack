using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nest;
using N5.Permissions.Domain.Entities;
using N5.Permissions.Domain.Interfaces;
using N5.Permissions.Infrastructure.Persistence;

namespace N5.Permissions.IntegrationTests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover el DbContext de SQL Server
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Usar base de datos en memoria para tests
            var dbName = "TestDb_" + Guid.NewGuid().ToString();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });

            // Remover servicios externos
            var elasticsearchDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IElasticClient));
            if (elasticsearchDescriptor != null)
                services.Remove(elasticsearchDescriptor);

            var elasticsearchServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IElasticsearchService));
            if (elasticsearchServiceDescriptor != null)
                services.Remove(elasticsearchServiceDescriptor);

            var kafkaServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IKafkaProducerService));
            if (kafkaServiceDescriptor != null)
                services.Remove(kafkaServiceDescriptor);

            // Registrar mocks
            var mockElasticsearchService = new Mock<IElasticsearchService>();
            services.AddSingleton(mockElasticsearchService.Object);

            var mockKafkaService = new Mock<IKafkaProducerService>();
            services.AddSingleton(mockKafkaService.Object);

            // Inicializar base de datos con datos de prueba
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            context.Database.EnsureCreated();
            
            if (!context.PermissionTypes.Any())
            {
                context.PermissionTypes.AddRange(
                    new PermissionType { Id = 1, Descripcion = "Enfermedad" },
                    new PermissionType { Id = 2, Descripcion = "Tramite" },
                    new PermissionType { Id = 3, Descripcion = "Mudanza" },
                    new PermissionType { Id = 4, Descripcion = "Vacaciones" }
                );
                context.SaveChanges();
            }
        });
    }
}

