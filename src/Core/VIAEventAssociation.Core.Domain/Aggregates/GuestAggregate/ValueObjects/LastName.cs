using System.Text.RegularExpressions;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

public record LastName
{
    internal string Value { get; }
    
    private LastName(string input) => Value = FormatName(input);
    
    public static Result<LastName> Create(string lastName) => 
        string.IsNullOrWhiteSpace(lastName)
            ? GuestErrors.LastName.LastNameIsEmpty : Validate(lastName);
    
    private static Result<LastName> Validate(string lastName) =>
        ResultExtensions.AssertAll(
            () => MustHaveValidLength(lastName),
            () => MustContainOnlyLetters(lastName)
        ).WithPayloadIfSuccess(() => new LastName(lastName));

    private static Result<None> MustHaveValidLength(string firstName)
    {
        if (firstName.Length < 2 || firstName.Length > 25)
            return GuestErrors.LastName.InvalidLength;

        return new None();
    }
    
    private static Result<None> MustContainOnlyLetters(string firstName)
    {
        Regex regex = new(@"^[a-zA-Z]+$");
        Match match = regex.Match(firstName);
        if (!match.Success)
            return GuestErrors.LastName.InvalidCharacters;

        return new None();
    }

    private static string FormatName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        return char.ToUpper(name[0]) + name.Substring(1).ToLower();
    }
}