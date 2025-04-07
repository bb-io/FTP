using Apps.FTP.Api;
using Apps.FTP.Dtos;
using Apps.FTP.Models.Responses;
using Apps.FTP.Webhooks.Payload;
using Apps.FTP.Webhooks.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using FluentFTP;

namespace Apps.FTP.Webhooks
{
    [PollingEventList]
    public class PollingList(InvocationContext invocationContext) : FTPInvocable(invocationContext)
    {
        [PollingEvent("On directories created", Description = "Triggers when directories are created")]
        public async Task<PollingEventResponse<FTPDirectoryMemory, ListDirectoryResponse>> OnDirectoriesCreated(
            PollingEventRequest<FTPDirectoryMemory> request,
            [PollingEventParameter] ParentFolderInput parentFolder)
        {
            using var client = new FTPClient(Creds);
            await client.Connect();
            var directories = await ListDirectoryFolders(client, parentFolder.Folder ?? "/",
                parentFolder.IncludeSubfolders ?? false);

            var directoryState = directories.Select(x => x.FullName).ToList();
            if (request.Memory == null)
            {            
                return new()
                {
                    FlyBird = false,
                    Memory = new FTPDirectoryMemory { DirectoriesState = directoryState }
                };
            }
            
            var newItems = directoryState.Except(request.Memory.DirectoriesState).ToList();
            if (newItems.Count() == 0)
            {
                return new()
                {
                    FlyBird = false,
                    Memory = new FTPDirectoryMemory { DirectoriesState = directoryState }
                };
            }
            
            return new()
            {
                FlyBird = true,
                Memory = new FTPDirectoryMemory { DirectoriesState = directoryState },
                Result = new ListDirectoryResponse
                {
                    DirectoriesItems = directories.Where(x => newItems.Contains(x.FullName)).Select(x => new DirectoryItemDto
                    {
                        Name = x.Name,
                        Path = x.FullName
                    })
                }
            };
        }
        
        [PollingEvent("On files created or updated", "On files created or updated")]
        public async Task<PollingEventResponse<FTPMemory, ChangedFilesResponse>> OnFilesAddedOrUpdated(
            PollingEventRequest<FTPMemory> request,
            [PollingEventParameter] ParentFolderInput parentFolder
            )
        {
            using var client = new FTPClient(Creds);
            await client.Connect();
            var filesInfo = await ListDirectoryFiles(client, parentFolder.Folder ?? "/", parentFolder.IncludeSubfolders ?? false);
            var newFilesState = filesInfo.Select(x => $"{x.FullName}|{x.Modified}").ToList();
            if (request.Memory == null)
            {            
                return new()
                {
                    FlyBird = false,
                    Memory = new FTPMemory() { FilesState = newFilesState }
                };
            }
            var changedItems = newFilesState.Except(request.Memory.FilesState).ToList();
            if (changedItems.Count == 0)
                return new()
                {
                    FlyBird = false,
                    Memory = new FTPMemory() { FilesState = newFilesState }
                };
            var changedFilesPath = changedItems.Select(x => x.Split('|').First()).ToList();
            return new()
            {
                FlyBird = true,
                Memory = new FTPMemory() { FilesState = newFilesState },
                Result = new ChangedFilesResponse() { Files = filesInfo.Where(x => changedFilesPath.Contains(x.FullName)).Select(x => new DirectoryItemDto() { Name = x.Name, Path = x.FullName }).ToList() }
            };
        }

        [PollingEvent("On files deleted", "On files deleted")]
        public async Task<PollingEventResponse<FTPMemory, ChangedFilesResponse>> OnFilesDeleted(
            PollingEventRequest<FTPMemory> request,
            [PollingEventParameter] ParentFolderInput parentFolder
            )
        {
            using var client = new FTPClient(Creds);
            await client.Connect();
            var filesInfo = await ListDirectoryFiles(client, parentFolder.Folder ?? "/", parentFolder.IncludeSubfolders ?? false);
            var newFilesState = filesInfo.Select(x => $"{x.FullName}").ToList();
            if (request.Memory == null)
            {
                return new()
                {
                    FlyBird = false,
                    Memory = new FTPMemory() { FilesState = newFilesState }
                };
            }
            var deletedItems = request.Memory.FilesState.Except(newFilesState).ToList();
            if (deletedItems.Count == 0)
                return new()
                {
                    FlyBird = false,
                    Memory = new FTPMemory() { FilesState = newFilesState }
                };
            return new()
            {
                FlyBird = true,
                Memory = new FTPMemory() { FilesState = newFilesState },
                Result = new ChangedFilesResponse() { Files = deletedItems.Select(x => new DirectoryItemDto() { Name = Path.GetFileName(x), Path = x }).ToList() }
            };
        }

        private async Task<List<FtpListItem>> ListDirectoryFiles(FTPClient FTPClient, string folderPath, bool includeSubfolder)
        {
            if (includeSubfolder)
            {
                var directories = (await FTPClient.GetListing(folderPath, FtpListOption.Recursive)).Where(x => x.Type == FtpObjectType.File).ToList();
                return directories;
            }
            else
            {
                var directories = (await FTPClient.GetListing(folderPath)).Where(x => x.Type == FtpObjectType.File).ToList();
                return directories;
            }
        }


        private async Task<List<FtpListItem>> ListDirectoryFolders(FTPClient FTPClient, string folderPath, bool includeSubfolder)
        {
            if (includeSubfolder)
            {
                var directories = (await FTPClient.GetListing(folderPath, FtpListOption.Recursive)).Where(x => x.Type == FtpObjectType.Directory).ToList();
                return directories;
            }
            else
            {
                var directories = (await FTPClient.GetListing(folderPath)).Where(x => x.Type == FtpObjectType.Directory).ToList();
                return directories;
            }
        }
    }
}
