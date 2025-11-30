using N5.Permissions.Domain.Enums;

namespace N5.Permissions.Domain.Interfaces;

public interface IKafkaProducerService
{
    Task PublishOperationAsync(OperationType operationType);
}

