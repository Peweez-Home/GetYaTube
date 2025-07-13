using System;
using System.IO;

namespace GetYaTube.Services;

public class DesktopPathService : IPathService
{
    public string GetDefaultDownloadPath()
    {
        // Gets the user's "Downloads" folder
        string? userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
        if (string.IsNullOrEmpty(userProfile))
            return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        
        return Path.Combine(userProfile, "Downloads");
    }
}