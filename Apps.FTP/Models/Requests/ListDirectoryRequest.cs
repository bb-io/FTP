using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.FileStorage;

namespace Apps.FTP.Models.Requests;

public class ListDirectoryRequest
{
    [Display("Folder path")]
    public string? FolderId { get; set; }
}