namespace N5.Permissions.Application.Interfaces;

public interface IQueryHandler<TResult>
{
    Task<TResult> Handle();
}

