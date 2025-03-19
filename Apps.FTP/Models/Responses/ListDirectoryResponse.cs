using Apps.FTP.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Responses;

public class ListDirectoryResponse
{
    [Display("Directory items")]
    public IEnumerable<DirectoryItemDto> DirectoriesItems { get; set; }
}