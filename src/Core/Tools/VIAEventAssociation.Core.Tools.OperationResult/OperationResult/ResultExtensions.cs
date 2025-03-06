namespace VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

public static class ResultExtensions
{
    public static Result<T> ToResult<T>(this T value)
    {
        if (value == null)
            return new Failure<T>(new[]{ new ResultError("NULL","Value is null") });
        return value; // implicit -> new Success<T>(value)
    }

    public static Result<U> Map<T,U>(this Result<T> result, Func<T,U> transform)
    {
        return result switch
        {
            Success<T> s => new Success<U>(transform(s.Value)),
            Failure<T> f => new Failure<U>(f.Errors),
            _ => throw new NotSupportedException()
        };
    }

    public static Result<T> Where<T>(this Result<T> result, 
        Func<T,bool> predicate, ResultError errorIfFalse)
    {
        return result switch
        {
            Success<T> s => predicate(s.Value)
                ? s
                : new Failure<T>(new[]{ errorIfFalse }),
            Failure<T> f => f,
            _ => throw new NotSupportedException()
        };
    }

    public static Result<U> Match<T,U>(this Result<T> result,
        Func<T,U> onSuccess,
        Func<IEnumerable<ResultError>,U> onFailure)
    {
        return result switch
        {
            Success<T> s => onSuccess(s.Value),
            Failure<T> f => onFailure(f.Errors),
            _ => throw new NotSupportedException()
        };
    }
    
    public static Result<None> AssertAll(params Func<Result<None>>[] validations)
    {
        var errors = new List<ResultError>();
        foreach (var check in validations)
        {
            var result = check();
            if (result is Failure<None> f)
            {
                errors.AddRange(f.Errors);
            }
        }

        return errors.Count == 0
            ? new Success<None>(new None())
            : new Failure<None>(errors);
    }
    
    public static Result<T> WithPayloadIfSuccess<T>(this Result<None> result, Func<T> createPayload)
    {
        return result switch
        {
            Success<None> => new Success<T>(createPayload()),
            Failure<None> f => new Failure<T>(f.Errors),
            _ => throw new NotSupportedException()
        };
    }

}
