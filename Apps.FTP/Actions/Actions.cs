using Apps.FTP.Api;
using Apps.FTP.Dtos;
using Apps.FTP.Models.Requests;
using Apps.FTP.Models.Responses;
using Apps.FTP.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using FluentFTP.Exceptions;

namespace Apps.FTP.Actions;

[ActionList]
public class Actions : FTPInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public Actions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Upload file", Description = "Uploads a file to the FTP server")]
    public async Task UploadFile([ActionParameter] UploadFileRequest uploadFileRequest)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());

        string path = string.Empty; 
        string fileName = string.Empty;
        if (string.IsNullOrEmpty(uploadFileRequest.FileName))
        {
            fileName = uploadFileRequest.File.Name;
        }

        if(!string.IsNullOrEmpty(uploadFileRequest.Path))
        {
            path = uploadFileRequest.Path;
        }

        using (var file = await _fileManagementClient.DownloadAsync(uploadFileRequest.File))
        {

            await Client.UploadStream(file, $"{path}/{fileName}");
        }
    }

    [Action("Download file", Description = "Downloads a file from the FTP server")]
    public async Task<DownloadFileResponse> DownloadFile([ActionParameter] DownloadFileRequest downloadFileRequest)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());

        using (var stream = new MemoryStream())
        {
            try
            {
                await Client.DownloadStream(stream, downloadFileRequest.Path);
            }
            catch (ArgumentException ex)
            {
                throw new PluginMisconfigurationException(ex.Message);
            }
            
            stream.Position = 0;
            var mimeType = "application/octet-stream";

            if (stream.Length == 0)
            {
                throw new PluginMisconfigurationException("The file cannot be found.");
            }

            var fileReference = await _fileManagementClient.UploadAsync(stream, mimeType, Path.GetFileName(downloadFileRequest.Path));

            return new DownloadFileResponse
            {
                File = fileReference
            };
        }
            
    }

    [Action("Search files", Description = "Searches for files in a directory on the FTP server")]
    public async Task<ListDirectoryResponse> ListDirectory([ActionParameter]ListDirectoryRequest listDirectoryRequest)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());
        var listings = await Client.GetListing(listDirectoryRequest.Path, FluentFTP.FtpListOption.Recursive);

        var items = listings.Select(i => new DirectoryItemDto()
        {
            Name = i.Name,
            Path = i.FullName
        });

        return new ListDirectoryResponse()
        {
            DirectoriesItems = items
        };
    }

    [Action("Delete file", Description = "Deletes a file from the FTP server")]
    public async Task DeleteFile([ActionParameter] [Display("Remote file path")] string remoteFilePath)
    {
        if (String.IsNullOrEmpty(remoteFilePath))
        {
            throw new PluginMisconfigurationException("Please enter a valid path");
        }

        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());
        try
        {
            await Client.DeleteFile(remoteFilePath);
        }
        catch (FtpCommandException ex)
        {

            throw new PluginApplicationException(ex.Message);
        }
        
    }

    [Action("Rename file", Description = "Rename a path from old to new")]
    public async Task RenameFile([ActionParameter] RenameFileRequest input)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());
        try
        {
            await Client.Rename(input.OldPath, input.NewPath);
        }
        catch (ArgumentException ex)
        {
            throw new PluginMisconfigurationException(ex.Message);
        }
        catch(FtpCommandException ex)
        {
            throw new PluginApplicationException(ex.Message);
        }
        
    }

    [Action("Create directory", Description = "Create new directory by path")]
    public async Task CreateDirectory([ActionParameter] CreateDirectoryRequest input)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async ()=> await Client.Connect());

        string directory;

        if (input.Path==null)
        {
            directory = input.DirectoryName;
        }
        else
        {
            directory = $"{input.Path}/{input.DirectoryName}";
        }

        await Client.CreateDirectory(directory);
    }

    [Action("Delete directory", Description = "Delete directory by path")]
    public async Task DeleteDirectory([ActionParameter] DeleteDirectoryRequest input)
    {
        if (String.IsNullOrEmpty(input.Path))
        {
            throw new PluginMisconfigurationException("Please enter a valid path");
        }
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());

        await Client.DeleteDirectory(input.Path);
    }
}
