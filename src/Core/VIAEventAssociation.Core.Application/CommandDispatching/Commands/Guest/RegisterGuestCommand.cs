using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;

public class RegisterGuestCommand
{
    public ViaEmail Email { get; }
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public ProfilePictureUrl ProfilePictureUrl { get; }
    
    private RegisterGuestCommand(ViaEmail email, FirstName firstName, LastName lastName, ProfilePictureUrl profilePictureUrl)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ProfilePictureUrl = profilePictureUrl;
    }

    public static Result<RegisterGuestCommand> Create(string email, string firstName, string lastName, string profilePictureUrl)
    {
        var emailResult = ViaEmail.Create(email);
        var firstNameResult = FirstName.Create(firstName);
        var lastNameResult = LastName.Create(lastName);
        var profilePictureUrlResult = ProfilePictureUrl.Create(profilePictureUrl);

        return ResultExtensions.CombineResultsInto<RegisterGuestCommand>(
                emailResult, firstNameResult, lastNameResult, profilePictureUrlResult)
            .WithPayloadIfSuccess(() => new RegisterGuestCommand(
                emailResult.Value, 
                firstNameResult.Value, 
                lastNameResult.Value, 
                profilePictureUrlResult.Value));
    }
}