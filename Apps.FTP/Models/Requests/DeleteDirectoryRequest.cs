
using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Requests;

public class DeleteDirectoryRequest
{
    [Display("Folder path")]
    public string FolderId { get; set; }
}