using Apps.FTP.Models.Requests;
using Apps.FTP.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.FTP.Actions
{
    [ActionList("Folders")]
    public class FolderActions : FTPInvocable
    {
        private readonly IFileManagementClient _fileManagementClient;

        public FolderActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
        {
            _fileManagementClient = fileManagementClient;
        }

        [Action("Create folder", Description = "Create new folder by path")]
        public async Task CreateDirectory([ActionParameter] CreateDirectoryRequest input)
        {
            await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());

            string directory;

            if (input.ParentFolderId == null)
            {
                directory = input.FolderName;
            }
            else
            {
                directory = $"{input.ParentFolderId}/{input.FolderName}";
            }

            await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.CreateDirectory(directory));
        }

        [Action("Delete folder", Description = "Delete folder by path")]
        public async Task DeleteDirectory([ActionParameter] DeleteDirectoryRequest input)
        {
            if (String.IsNullOrEmpty(input.FolderId))
            {
                throw new PluginMisconfigurationException("Please enter a valid path");
            }
            await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.Connect());

            await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.DeleteDirectory(input.FolderId));
        }
    }
}
