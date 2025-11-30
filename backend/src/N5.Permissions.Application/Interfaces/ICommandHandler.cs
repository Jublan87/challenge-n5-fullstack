namespace N5.Permissions.Application.Interfaces;

public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command);
}

