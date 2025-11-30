using AutoMapper;
using N5.Permissions.Application.DTOs;
using N5.Permissions.Application.Interfaces;
using N5.Permissions.Domain.Enums;
using N5.Permissions.Domain.Interfaces;

namespace N5.Permissions.Application.Queries;

public class GetPermissionsHandler : IQueryHandler<IReadOnlyList<PermissionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IKafkaProducerService _kafkaProducerService;

    public GetPermissionsHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IKafkaProducerService kafkaProducerService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task<IReadOnlyList<PermissionDto>> Handle()
    {
        var permissions = await _unitOfWork.Permissions.GetAllAsync();
        await _kafkaProducerService.PublishOperationAsync(OperationType.Get);
        return _mapper.Map<IReadOnlyList<PermissionDto>>(permissions);
    }
}


