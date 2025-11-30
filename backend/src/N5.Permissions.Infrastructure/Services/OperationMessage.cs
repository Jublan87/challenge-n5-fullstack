using N5.Permissions.Domain.Enums;

namespace N5.Permissions.Infrastructure.Services;

public class OperationMessage
{
    public Guid Id { get; set; }
    public OperationType NameOperation { get; set; }
}

