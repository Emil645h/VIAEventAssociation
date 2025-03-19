using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

public class ProfilePictureUrl
{
    internal Uri Value { get; }
    
    private ProfilePictureUrl(Uri url) => Value = url;

    public static Result<ProfilePictureUrl> Create(string url)
        => string.IsNullOrWhiteSpace(url) ? GuestErrors.ProfilePictureUrl.UrlIsEmpty : Validate(url);

    private static Result<ProfilePictureUrl> Validate(string url)
        => ResultExtensions.AssertAll(
            () => MustBeValidUrlFormat(url)
        ).WithPayloadIfSuccess(() => new ProfilePictureUrl(new Uri(url)));

    private static Result<None> MustBeValidUrlFormat(string url)
    {
        bool isValid = Uri.TryCreate(url, UriKind.Absolute, out Uri uri) && 
                       (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
                       
        if (!isValid)
        {
            return GuestErrors.ProfilePictureUrl.InvalidUrlFormat;
        }
        
        return new None();
    }
}