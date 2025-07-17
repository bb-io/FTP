using Apps.FTP.Api;
using Apps.FTP.Dtos;
using Apps.FTP.Models.Requests;
using Apps.FTP.Models.Responses;
using Apps.FTP.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.FTP.Actions;

[ActionList("Files")]
public class FileActions : FTPInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public FileActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [BlueprintActionDefinition(BlueprintAction.UploadFile)]
    [Action("Upload file", Description = "Uploads a file to the FTP server")]
    public async Task UploadFile([ActionParameter] UploadFileRequest uploadFileRequest)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());

        string path = string.Empty; 
        string fileName = uploadFileRequest.FileName; ;
        if (string.IsNullOrEmpty(uploadFileRequest.FileName))
        {
            fileName = uploadFileRequest.File.Name;
        }

        if (!string.IsNullOrEmpty(uploadFileRequest.FolderId))
        {
            path = uploadFileRequest.FolderId + "/";
        }

        using (var file = await _fileManagementClient.DownloadAsync(uploadFileRequest.File))
        {

            await Client.UploadStream(file, $"{path}{fileName}");
        }
    }

    [BlueprintActionDefinition(BlueprintAction.DownloadFile)]
    [Action("Download file", Description = "Downloads a file from the FTP server")]
    public async Task<DownloadFileResponse> DownloadFile([ActionParameter] DownloadFileRequest downloadFileRequest)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());

        using (var stream = new MemoryStream())
        {
            await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.DownloadStream(stream, downloadFileRequest.FileId));
            
            stream.Position = 0;
            var mimeType = "application/octet-stream";

            if (stream.Length == 0)
            {
                throw new PluginMisconfigurationException("The file cannot be found.");
            }

            var fileReference = await _fileManagementClient.UploadAsync(stream, mimeType, Path.GetFileName(downloadFileRequest.FileId));

            return new DownloadFileResponse
            {
                File = fileReference
            };
        }            
    }

    [Action("Search files", Description = "Searches for files in a directory on the FTP server")]
    public async Task<ListDirectoryResponse> ListDirectory([ActionParameter] ListDirectoryRequest listDirectoryRequest)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());
        var listings = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.GetListing(listDirectoryRequest.FolderId, FluentFTP.FtpListOption.Recursive));

        var items = listings.Select(i => new DirectoryItemDto()
        {
            Name = i.Name,
            FileId = i.FullName
        });

        return new ListDirectoryResponse()
        {
             Files = items
        };
    }

    [Action("Delete file", Description = "Deletes a file from the FTP server")]
    public async Task DeleteFile([ActionParameter] DeleteFileRequest file)
    {
        if (String.IsNullOrEmpty(file.FileId))
        {
            throw new PluginMisconfigurationException("Please enter a valid path");
        }

        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.DeleteFile(file.FileId));
        
    }

    [Action("Rename file", Description = "Rename a path from old to new")]
    public async Task RenameFile([ActionParameter] RenameFileRequest input)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Rename(input.OldPath, input.NewPath));
    }   
}
