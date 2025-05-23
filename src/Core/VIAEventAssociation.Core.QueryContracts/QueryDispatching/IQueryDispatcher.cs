using VIAEventAssociation.Core.QueryContracts.Contract;

namespace VIAEventAssociation.Core.QueryContracts.QueryDispatching;

public interface IQueryDispatcher
{
    Task<TAnswer> DispatchAsync<TAnswer>(IQuery<TAnswer> query);
}