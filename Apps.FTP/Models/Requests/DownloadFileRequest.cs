﻿using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Requests;

public class DownloadFileRequest
{
    [Display("Full path")]
    public string Path { get; set; }
}