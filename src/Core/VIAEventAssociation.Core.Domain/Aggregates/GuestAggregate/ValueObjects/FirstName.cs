using System.Text.RegularExpressions;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

public class FirstName
{
    internal string Value { get; }
    
    private FirstName(string input) => Value = FormatName(input);
    
    public static Result<FirstName> Create(string firstName) => 
    string.IsNullOrWhiteSpace(firstName)
        ? GuestErrors.FirstName.FirstNameIsEmpty : Validate(firstName);
    
    private static Result<FirstName> Validate(string firstName) =>
    ResultExtensions.AssertAll(
        () => MustHaveValidLength(firstName),
        () => MustContainOnlyLetters(firstName)
        ).WithPayloadIfSuccess(() => new FirstName(firstName));

    private static Result<None> MustHaveValidLength(string firstName)
    {
        if (firstName.Length < 2 || firstName.Length > 25)
            return GuestErrors.FirstName.InvalidLength;

        return new None();
    }
    
    private static Result<None> MustContainOnlyLetters(string firstName)
    {
        Regex regex = new(@"^[a-zA-Z]+$");
        Match match = regex.Match(firstName);
        if (!match.Success)
            return GuestErrors.FirstName.InvalidCharacters;

        return new None();
    }

    private static string FormatName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        return char.ToUpper(name[0]) + name.Substring(1).ToLower();
    }
}