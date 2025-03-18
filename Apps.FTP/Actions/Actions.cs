using Apps.FTP.Models.Requests;
using Apps.FTP.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using System.Net.Mime;
using System.Threading.Tasks;

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
        using (var stream = new MemoryStream())
        {
            await Client.DownloadStream(stream,downloadFileRequest.Path);
            stream.Position = 0;
            var mimeType = "application/octet-stream";

            var fileReference = await _fileManagementClient.UploadAsync(stream, mimeType, Path.GetFileName(downloadFileRequest.Path));

            return new DownloadFileResponse
            {
                File = fileReference
            };
        }
            
    }

    [Action("List Directory", Description = "Lists the contents of a directory on the FTP server")]
    public async Task<ListDirectoryResponse> ListDirectory(ListDirectoryRequest listDirectoryRequest)
    {
        throw new NotImplementedException();
    }

    [Action("Delete File", Description = "Deletes a file from the FTP server")]
    public async Task DeleteFile(string remoteFilePath)
    {
        await Client.DeleteFile(remoteFilePath);
    }

    [Action("Rename file", Description = "Rename a path from old to new")]
    public async Task RenameFile([ActionParameter] RenameFileRequest input)
    {
        await Client.Rename(input.OldPath, input.NewPath);
    }

    [Action("Create directory", Description = "Create new directory by path")]
    public async Task CreateDirectory([ActionParameter] CreateDirectoryRequest input)
    {
        await Client.CreateDirectory(input.Path);
    }

    [Action("Delete directory", Description = "Delete directory by path")]
    public async Task DeleteDirectory([ActionParameter] DeleteDirectoryRequest input)
    {
        await Client.DeleteDirectory(input.Path);
    }
}
