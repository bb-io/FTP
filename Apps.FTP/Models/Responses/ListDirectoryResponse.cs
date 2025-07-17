using Apps.FTP.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Responses;

public class ListDirectoryResponse
{
    [Display("Folder files")]
    public IEnumerable<DirectoryItemDto> Files { get; set; }
}