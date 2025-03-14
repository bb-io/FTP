using Apps.FTP.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using FluentFTP;
using RestSharp;

namespace Apps.FTP.Connections;

public class ConnectionValidator: IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            using (var client = new FTPClient(authenticationCredentialsProviders))
            {

                client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
                client.Config.ValidateAnyCertificate = true;
                await client.Connect(cancellationToken);

                if (client.IsConnected)
                    return new ConnectionValidationResponse
                    {
                        IsValid = true
                    };
                else
                {
                    return new ConnectionValidationResponse
                    {
                        IsValid = false,
                        Message = "Failed to connect. Please check your connection parameters."
                    };
                }
            }
        }
        catch (FormatException ex)
        {
            return new ConnectionValidationResponse()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new ConnectionValidationResponse()
            {
                IsValid = false,
                Message = ex.Message,
            };
        }

    }
}
