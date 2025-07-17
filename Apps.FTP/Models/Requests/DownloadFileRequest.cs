using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.FileStorage;

namespace Apps.FTP.Models.Requests;

public class DownloadFileRequest : IDownloadFileInput
{
    [Display("File path")]
    public string FileId { get; set; }
}