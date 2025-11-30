using AutoMapper;
using N5.Permissions.Application.DTOs;
using N5.Permissions.Application.Interfaces;
using N5.Permissions.Domain.Entities;
using N5.Permissions.Domain.Enums;
using N5.Permissions.Domain.Interfaces;

namespace N5.Permissions.Application.Commands;

public class RequestPermissionHandler : ICommandHandler<CreatePermissionDto, PermissionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly IKafkaProducerService _kafkaProducerService;

    public RequestPermissionHandler(
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

    public async Task<PermissionDto> Handle(CreatePermissionDto dto)
    {
        var tipoPermisoExists = await _unitOfWork.PermissionTypes.ExistsAsync(dto.TipoPermiso);
        if (!tipoPermisoExists)
            throw new ArgumentException($"Permission type {dto.TipoPermiso} does not exist");

        var permission = _mapper.Map<Permission>(dto);
        permission = await _unitOfWork.Permissions.AddAsync(permission);
        await _unitOfWork.SaveChangesAsync();

        permission = await _unitOfWork.Permissions.GetByIdAsync(permission.Id);
        if (permission == null)
            throw new InvalidOperationException("Permission not found after creation");

        await _elasticsearchService.IndexPermissionAsync(permission);
        await _kafkaProducerService.PublishOperationAsync(OperationType.Request);

        return _mapper.Map<PermissionDto>(permission);
    }
}


