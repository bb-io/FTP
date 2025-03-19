
using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Requests;

public class DeleteDirectoryRequest
{
    [Display("Directory path")]
    public string Path { get; set; }
}