using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;

public class CreateGuestCommand
{
    public GuestId Id { get; }
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public ViaEmail Email { get; }
    public ProfilePictureUrl ProfilePictureUrl { get; }

    private CreateGuestCommand(
        GuestId id,
        FirstName firstName,
        LastName lastName,
        ViaEmail email,
        ProfilePictureUrl profilePictureUrl)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        ProfilePictureUrl = profilePictureUrl;
    }

    public static Result<CreateGuestCommand> Create(
        string id,
        string firstName,
        string lastName,
        string email,
        string profilePictureUrl)
    {
        var idResult = GuestId.FromString(id);
        var firstNameResult = FirstName.FromString(firstName);
        var lastNameResult = LastName.FromString(lastName);
        var emailResult = ViaEmail.FromString(email);
        var profilePictureUrlResult = ProfilePictureUrl.FromString(profilePictureUrl);

        return Result<None>.Combine(
                idResult.Map(_ => new None()),
                firstNameResult.Map(_ => new None()),
                lastNameResult.Map(_ => new None()),
                emailResult.Map(_ => new None()),
                profilePictureUrlResult.Map(_ => new None())
            )
            .WithPayloadIfSuccess(() =>
                new CreateGuestCommand(
                    idResult.Value,
                    firstNameResult.Value,
                    lastNameResult.Value,
                    emailResult.Value,
                    profilePictureUrlResult.Value
                ));
    }
}