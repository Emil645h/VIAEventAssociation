namespace VIAEventAssociation.Core.Tools.OperationResult.CustomExceptions;

[Serializable]
public class ServiceNotFoundException : Exception
{
    public ServiceNotFoundException() : base("The requested service was not found.") { }
    
    public ServiceNotFoundException(string message) : base(message) { }
    
    public ServiceNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}