using Apps.FTP.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using FluentFTP;
using FluentFTP.Client.BaseClient;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.FTP.Api;

public class FTPClient : AsyncFtpClient
{

    public FTPClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders): base()
        { 
        Host = authenticationCredentialsProviders.First(p => p.KeyName == "host").Value;
        Port = Convert.ToInt32(authenticationCredentialsProviders.First(p => p.KeyName == "port").Value);
        Credentials.UserName = authenticationCredentialsProviders.First(p => p.KeyName == "username").Value;
        Credentials.Password = authenticationCredentialsProviders.First(p => p.KeyName == "password").Value;
}
}
