using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Requests;

public class ListDirectoryRequest
{
    [Display("Directory path")]
    public string? Path { get; set; }
}