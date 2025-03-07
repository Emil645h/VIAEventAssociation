using System.Text.RegularExpressions;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

public class FirstName
{
    internal string Value { get; }
    
    internal FirstName(string input) => Value = input;
    
    public static Result<FirstName> Create(string firstName) => 
    string.IsNullOrWhiteSpace(firstName)
        ? GuestErrors.FirstName.FirstNameIsEmpty : Validate(firstName);
    
    private static Result<FirstName> Validate(string firstName) =>
    ResultExtensions.AssertAll(
        () => MustContainValidCharacters(firstName)
        ).WithPayloadIfSuccess(() => new FirstName(firstName));

    private static Result<None> MustContainValidCharacters(string firstName)
    {
        Regex regex = new(@"^[\p{L}]+(?:[\s'-][\p{L}]+)*$");
        Match match = regex.Match(firstName);
        if (!match.Success)
            return GuestErrors.FirstName.InvalidCharacters;
        return new Success<None>(new None());
    }
}