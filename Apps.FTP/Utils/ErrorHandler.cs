using Blackbird.Applications.Sdk.Common.Exceptions;
using FluentFTP.Exceptions;

namespace Apps.FTP.Utils;

public static class ErrorHandler
{
    public static async Task ExecuteWithErrorHandlingAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (FtpAuthenticationException)
        {
            throw new PluginMisconfigurationException("Your FTP credentials are invalid. Please check your credentials or account status.");
        }
        catch (ArgumentException ex)
        {
            throw new PluginMisconfigurationException(ex.Message);
        }
        catch (FtpCommandException ex)
        {
            throw new PluginApplicationException(ex.Message);
        }
        catch (FtpException ex)
        {
            throw new PluginApplicationException(ex.Message);
        }
    }

    public static async Task<T> ExecuteWithErrorHandlingAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (FtpAuthenticationException)
        {
            throw new PluginMisconfigurationException("Your FTP credentials are invalid. Please check your credentials or account status.");
        }
        catch (ArgumentException ex)
        {
            throw new PluginMisconfigurationException(ex.Message);
        }
        catch (FtpCommandException ex)
        {
            throw new PluginApplicationException(ex.Message);
        }
        catch (FtpException ex)
        {
            throw new PluginApplicationException(ex.Message);
        }
    }
}