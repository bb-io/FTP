
using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Requests;

public class CreateDirectoryRequest
{
    [Display("Folder name")]
    public string FolderName { get; set; }

    [Display("Parent folder path", Description = "The path, '/' being the root  (default).")]
    public string? ParentFolderId { get; set; }
}
