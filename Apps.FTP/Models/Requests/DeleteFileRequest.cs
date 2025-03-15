using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Requests;

public class DeleteFileRequest
{
    [Display("Full path")]
    public string FilePath { get; set; }
}