namespace VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

public abstract record Result;

public abstract record Result<T> : Result
{
    public bool IsSuccess => this is Success<T>;
    public bool IsFailure => this is Failure<T>;
    
    public static implicit operator Result<T>(T value) 
        => new Success<T>(value);

    public static implicit operator Result<T>(ResultError error) 
        => new Failure<T>(new[] { error });
    
    public static Result<T> Combine(IEnumerable<Result<T>> results)
    {
        var errors = new List<ResultError>();
        T? lastValue = default;
        bool anySuccess = false;

        foreach (var r in results)
        {
            switch (r)
            {
                case Success<T> s:
                    lastValue = s.Value;
                    anySuccess = true;
                    break;
                case Failure<T> f:
                    errors.AddRange(f.Errors);
                    break;
            }
        }

        if (errors.Count > 0)
        {
            return new Failure<T>(errors);
        }
        if (!anySuccess)
        {
            // Edge case: no success + no errors => decide how to handle
            // Possibly return a Failure with "No success" or fallback value
            return new Failure<T>(new[] 
            {
                new ResultError("NO_SUCCESS", "No successful results found.")
            });
        }

        return new Success<T>(lastValue!);
    }

    // Overload to combine an arbitrary number of results easily
    public static Result<T> Combine(params Result<T>[] results)
        => Combine((IEnumerable<Result<T>>)results);
    
    
}

public record Success<T>(T Value) : Result<T>;

public record Failure<T>(IEnumerable<ResultError> Errors) : Result<T>;

public record ResultError(string Code, string Message);

public record None : Result;

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
