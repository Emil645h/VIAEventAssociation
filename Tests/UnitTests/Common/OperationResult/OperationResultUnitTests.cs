using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.OperationResult;

public class OperationResultUnitTests
{
    [Fact]
    public void ImplicitOperator_FromValue_ShouldCreateSuccess()
    {
        // Arrange
        int number = 42;
        
        // Act
        Result<int> result = number; // implicit => Success<int>(42)

        // Assert
        Assert.IsType<Success<int>>(result);
        var success = (Success<int>)result;
        Assert.Equal(42, success.Value);
    }

    [Fact]
    public void ImplicitOperator_FromError_ShouldCreateFailure()
    {
        // Arrange
        var err = new ResultError("ERR001", "Something went wrong");

        // Act
        Result<string> result = err; // implicit => Failure<string>(...)

        // Assert
        Assert.IsType<Failure<string>>(result);
        var failure = (Failure<string>)result;
        var singleError = Assert.Single(failure.Errors);
        Assert.Equal("ERR001", singleError.Code);
        Assert.Equal("Something went wrong", singleError.Message);
    }

    [Fact]
    public void Combine_AllSuccess_ShouldReturnLastSuccess()
    {
        // Arrange
        var r1 = (Result<int>)10;
        var r2 = (Result<int>)20;
        var r3 = (Result<int>)30;

        // Act
        var combined = Result<int>.Combine(r1, r2, r3);

        // Assert
        Assert.IsType<Success<int>>(combined);
        var success = (Success<int>)combined;
        Assert.Equal(30, success.Value);
    }

    [Fact]
    public void Combine_AnyFailure_ShouldAggregateErrors()
    {
        // Arrange
        var success1 = (Result<int>)10;
        var failure = (Result<int>)new ResultError("E1", "First error");
        var anotherFailure = (Result<int>)new Failure<int>(new[]
        {
            new ResultError("E2", "Second error"),
            new ResultError("E3", "Third error")
        });

        // Act
        var combined = Result<int>.Combine(success1, failure, anotherFailure);

        // Assert
        Assert.IsType<Failure<int>>(combined);
        var allErrors = ((Failure<int>)combined).Errors.ToList();

        // Check we have all errors from both failures
        Assert.Equal(3, allErrors.Count);
        Assert.Contains(allErrors, e => e.Code == "E1" && e.Message == "First error");
        Assert.Contains(allErrors, e => e.Code == "E2" && e.Message == "Second error");
        Assert.Contains(allErrors, e => e.Code == "E3" && e.Message == "Third error");
    }

    [Fact]
    public void Combine_NoSuccessAndNoErrors_ShouldReturnAFailure()
    {
        // Arrange
        var emptyResults = new Result<int>[] { };

        // Act
        var combined = Result<int>.Combine(emptyResults);

        // Assert
        // We decided in our logic that if there's no success or no errors, 
        // we return a "NO_SUCCESS" error.
        Assert.IsType<Failure<int>>(combined);
        var fail = (Failure<int>)combined;
        var singleError = Assert.Single(fail.Errors);
        Assert.Equal("NO_SUCCESS", singleError.Code);
        Assert.Equal("No successful results found.", singleError.Message);
    }

    [Fact]
    public void SuccessRecord_CanBePatternMatched()
    {
        // Arrange
        Result<int> r = 99;

        // Act + Assert
        switch (r)
        {
            case Success<int> s:
                Assert.Equal(99, s.Value);
                break;
            default:
                Assert.False(true, "Expected a success, but got failure");
                break;
        }
    }

    [Fact]
    public void FailureRecord_CanBePatternMatched()
    {
        // Arrange
        Result<int> r = new ResultError("ERR002", "Breaking");

        // Act + Assert
        switch (r)
        {
            case Failure<int> f:
                Assert.Single(f.Errors);
                Assert.Equal("ERR002", f.Errors.First().Code);
                Assert.Equal("Breaking", f.Errors.First().Message);
                break;
            default:
                Assert.False(true, "Expected a failure, but got success");
                break;
        }
    }
}