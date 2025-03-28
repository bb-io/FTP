﻿using Blackbird.Applications.Sdk.Common;

namespace Apps.FTP.Models.Requests;

public class RenameFileRequest
{
    [Display("Old full path")]
    public string OldPath { get; set; }

    [Display("New full path")]
    public string NewPath { get; set; }
}