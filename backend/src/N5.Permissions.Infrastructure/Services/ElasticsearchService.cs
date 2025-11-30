using Nest;
using N5.Permissions.Domain.Entities;
using N5.Permissions.Domain.Interfaces;

namespace N5.Permissions.Infrastructure.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _client;
    private const string IndexName = "permissions";

    public ElasticsearchService(IElasticClient client)
    {
        _client = client;
    }

    private async Task EnsureIndexExistsAsync()
    {
        var indexExists = await _client.Indices.ExistsAsync(IndexName);
        
        if (!indexExists.Exists)
        {
            var createIndexResponse = await _client.Indices.CreateAsync(IndexName, c => c
                .Map<PermissionDocument>(m => m
                    .Properties(p => p
                        .Number(n => n.Name(per => per.Id))
                        .Text(t => t.Name(per => per.NombreEmpleado))
                        .Text(t => t.Name(per => per.ApellidoEmpleado))
                        .Number(n => n.Name(per => per.TipoPermiso))
                        .Date(d => d.Name(per => per.FechaPermiso))
                    )
                )
            );

            if (!createIndexResponse.IsValid)
                throw new Exception($"Failed to create Elasticsearch index: {createIndexResponse.DebugInformation}");
        }
    }

    public async Task IndexPermissionAsync(Permission permission)
    {
        await EnsureIndexExistsAsync();

        var document = new PermissionDocument
        {
            Id = permission.Id,
            NombreEmpleado = permission.NombreEmpleado,
            ApellidoEmpleado = permission.ApellidoEmpleado,
            TipoPermiso = permission.TipoPermiso,
            FechaPermiso = permission.FechaPermiso
        };

        var response = await _client.IndexAsync(document, i => i
            .Index(IndexName)
            .Id(permission.Id)
        );

        if (!response.IsValid)
            throw new Exception($"Failed to index permission: {response.DebugInformation}");
    }

    public async Task UpdatePermissionAsync(Permission permission)
    {
        await EnsureIndexExistsAsync();

        var document = new PermissionDocument
        {
            Id = permission.Id,
            NombreEmpleado = permission.NombreEmpleado,
            ApellidoEmpleado = permission.ApellidoEmpleado,
            TipoPermiso = permission.TipoPermiso,
            FechaPermiso = permission.FechaPermiso
        };

        var response = await _client.UpdateAsync<PermissionDocument>(permission.Id, u => u
            .Index(IndexName)
            .Doc(document)
        );

        if (!response.IsValid)
            throw new Exception($"Failed to update permission: {response.DebugInformation}");
    }

    public async Task<bool> DeletePermissionAsync(int id)
    {
        var response = await _client.DeleteAsync<PermissionDocument>(id, d => d
            .Index(IndexName)
        );

        return response.IsValid;
    }
}

