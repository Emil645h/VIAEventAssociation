namespace VIAEventAssociation.Core.Tools.OperationResult.CustomExceptions;

public class QueryHandlerNotFoundException : Exception
{
    public QueryHandlerNotFoundException(string queryType, string answerType) 
        : base($"No handler found for query type: {queryType} with answer type: {answerType}")
    {
    }
}