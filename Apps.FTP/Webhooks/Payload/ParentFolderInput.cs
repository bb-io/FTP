using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Webhooks.Payload
{
    public class ParentFolderInput
    {
        [Display("Parent folder path")]
        public string? FolderId { get; set; }

        [Display("Include subfolders")]
        public bool? IncludeSubfolders { get; set; }
    }
}
