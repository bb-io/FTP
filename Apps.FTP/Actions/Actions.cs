using Apps.FTP.Models.Requests;
using Apps.FTP.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.FTP.Actions;

[ActionList]
public class Actions : FTPInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public Actions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Upload File", Description = "Uploads a file to the FTP server")]
    public async Task UploadFile(UploadFileRequest uploadFileRequest)
    {
       
        using (var file = await _fileManagementClient.DownloadAsync(uploadFileRequest.File))
        {

            await Client.UploadStream(file, uploadFileRequest.Path);
        }
    }

    [Action("Download File", Description = "Downloads a file from the FTP server")]
    public async Task<DownloadFileResponse> DownloadFile(DownloadFileRequest downloadFileRequest)
    {
        throw new NotImplementedException();
    }

    [Action("List Directory", Description = "Lists the contents of a directory on the FTP server")]
    public async Task<ListDirectoryResponse> ListDirectory(ListDirectoryRequest listDirectoryRequest)
    {
        throw new NotImplementedException();
    }

    [Action("Delete File", Description = "Deletes a file from the FTP server")]
    public async Task DeleteFile(string remoteFilePath)
    {
        throw new NotImplementedException();
    }

    [Action("Rename file", Description = "Rename a path from old to new")]
    public void RenameFile([ActionParameter] RenameFileRequest input)
    {
        throw new NotImplementedException();
    }

    [Action("Create directory", Description = "Create new directory by path")]
    public void CreateDirectory([ActionParameter] CreateDirectoryRequest input)
    {
        throw new NotImplementedException();
    }

    [Action("Delete directory", Description = "Delete directory by path")]
    public void DeleteDirectory([ActionParameter] DeleteDirectoryRequest input)
    {
        throw new NotImplementedException();
    }
}
