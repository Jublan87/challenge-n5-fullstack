using AutoMapper;
using N5.Permissions.Application.DTOs;
using N5.Permissions.Application.Interfaces;
using N5.Permissions.Domain.Enums;
using N5.Permissions.Domain.Interfaces;

namespace N5.Permissions.Application.Commands;

public class ModifyPermissionHandler : ICommandHandler<(int id, UpdatePermissionDto dto), PermissionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly IKafkaProducerService _kafkaProducerService;

    public ModifyPermissionHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IElasticsearchService elasticsearchService,
        IKafkaProducerService kafkaProducerService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _elasticsearchService = elasticsearchService;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task<PermissionDto?> Handle((int id, UpdatePermissionDto dto) command)
    {
        var (id, dto) = command;
        var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
        if (permission is null)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.NombreEmpleado))
            permission.NombreEmpleado = dto.NombreEmpleado;

        if (!string.IsNullOrWhiteSpace(dto.ApellidoEmpleado))
            permission.ApellidoEmpleado = dto.ApellidoEmpleado;

        if (dto.TipoPermiso.HasValue)
        {
            var tipoPermisoExists = await _unitOfWork.PermissionTypes.ExistsAsync(dto.TipoPermiso.Value);
            if (!tipoPermisoExists)
                throw new ArgumentException($"Permission type {dto.TipoPermiso.Value} does not exist");
            
            permission.TipoPermiso = dto.TipoPermiso.Value;
        }

        if (dto.FechaPermiso.HasValue)
            permission.FechaPermiso = dto.FechaPermiso.Value;

        permission = await _unitOfWork.Permissions.UpdateAsync(permission);
        await _unitOfWork.SaveChangesAsync();

        permission = await _unitOfWork.Permissions.GetByIdAsync(permission.Id);
        if (permission == null)
            return null;

        await _elasticsearchService.UpdatePermissionAsync(permission);
        await _kafkaProducerService.PublishOperationAsync(OperationType.Modify);

        return _mapper.Map<PermissionDto>(permission);
    }
}


