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