using Apps.FTP.Dtos;

namespace Apps.FTP.Webhooks.Payload
{
    public class ChangedFilesResponse
    {
        public List<DirectoryItemDto> Files { get; set; }
    }
}
