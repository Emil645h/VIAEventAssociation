using System.Text.RegularExpressions;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

public record ViaEmail
{
    internal string Value { get; }
    
    internal ViaEmail(string input) => Value = input;

    public static Result<ViaEmail> Create(string email) =>
        email == null
            ? GuestErrors.ViaEmail.EmailIsEmpty
            : Validate(email);

    private static Result<ViaEmail> Validate(string email) =>
        ResultExtensions.AssertAll(
            () => MustNotBeNullOrEmpty(email),
            () => MustBeViaEmail(email),
            () => MustBeValidEmailStructure(email)
        ).WithPayloadIfSuccess(() => new ViaEmail(email));
    
    private static Result<None> MustNotBeNullOrEmpty(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return GuestErrors.ViaEmail.EmailIsEmpty;
        return new Success<None>(new None());
    }

    private static Result<None> MustBeViaEmail(string email)
    {
        if (!email.Contains("@via.dk", StringComparison.OrdinalIgnoreCase) && !email.Contains("@viauc.dk", StringComparison.OrdinalIgnoreCase))
            return GuestErrors.ViaEmail.MustViaEmail;

        return new Success<None>(new None());
    }

    private static Result<None> MustBeValidEmailStructure(string email)
    {
        Regex regex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(email);
        if (!match.Success)
            return GuestErrors.ViaEmail.InvalidEmailStructure;
        return new Success<None>(new None());
    }
    
}