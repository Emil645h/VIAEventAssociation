using System.Text.RegularExpressions;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

public class LastName
{
    internal string Value { get; }
    
    internal LastName(string input) => Value = input;
    
    public static Result<LastName> Create(string lastName) => 
        string.IsNullOrWhiteSpace(lastName)
            ? GuestErrors.LastName.LastNameIsEmpty : Validate(lastName);
    
    private static Result<LastName> Validate(string lastName) =>
        ResultExtensions.AssertAll(
            () => MustContainValidCharacters(lastName)
        ).WithPayloadIfSuccess(() => new LastName(lastName));

    private static Result<None> MustContainValidCharacters(string lastName)
    {
        Regex regex = new(@"^[\p{L}]+(?:[\s'-][\p{L}]+)*$");
        Match match = regex.Match(lastName);
        if (!match.Success)
            return GuestErrors.LastName.InvalidCharacters;
        return new Success<None>(new None());
    }
}