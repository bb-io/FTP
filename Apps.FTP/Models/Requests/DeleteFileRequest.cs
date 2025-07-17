using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Requests;

public class DeleteFileRequest
{
    [Display("File path")]
    public string FileId { get; set; }
}