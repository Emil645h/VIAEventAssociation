using System.Text.RegularExpressions;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

public record ViaEmail
{
    internal string Value { get; }
    
    private ViaEmail(string input) => Value = input.ToLower();

    public static Result<ViaEmail> Create(string email) =>
        email == null
            ? GuestErrors.ViaEmail.EmailIsEmpty
            : Validate(email);

    private static Result<ViaEmail> Validate(string email) =>
        ResultExtensions.AssertAll(
            () => MustNotBeNullOrEmpty(email),
            () => MustBeViaEmail(email),
            () => MustBeValidEmailStructure(email),
            () => MustHaveValidUsername(email)
        ).WithPayloadIfSuccess(() => new ViaEmail(email));
    
    private static Result<None> MustNotBeNullOrEmpty(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return GuestErrors.ViaEmail.EmailIsEmpty;
        
        return new None();
    }

    private static Result<None> MustBeViaEmail(string email)
    {
        if (!email.Contains("@via.dk", StringComparison.OrdinalIgnoreCase))
            return GuestErrors.ViaEmail.MustViaEmail;

        return new None();
    }

    private static Result<None> MustBeValidEmailStructure(string email)
    {
        Regex regex = new(@"^([^\s@]+)@([^\s@]+)\.([^\s@]+)$");
        Match match = regex.Match(email);
        if (!match.Success)
            return GuestErrors.ViaEmail.InvalidEmailStructure;

        return new None();
    }

    private static Result<None> MustHaveValidUsername(string email)
    {
        string username = email.Split('@')[0];
        
        // Username must be either:
        // - 3 or 4 letters (a-z, A-Z)
        // - 6 digits (0-9)
        bool isValid = (username.Length == 3 || username.Length == 4) && Regex.IsMatch(username, @"^[a-zA-Z]+$") ||
                       username.Length == 6 && Regex.IsMatch(username, @"^[0-9]+$");

        if (!isValid)
            return GuestErrors.ViaEmail.InvalidUsernameFormat;
        
        return new None();
    }
}