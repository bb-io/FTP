using Apps.FTP.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.FTP;

public class FTPInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected FTPClient Client { get; }
    public FTPInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds); // TODO check for config errors.
    }
}
