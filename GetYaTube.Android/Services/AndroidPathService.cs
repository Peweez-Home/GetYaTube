using Android.OS;
using GetYaTube.Services;

namespace GetYaTube.Android.Services;

public class AndroidPathService : IPathService
{
    public string GetDefaultDownloadPath()
    {
        // Gets the public "Music" directory on Android
        return Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMusic).AbsolutePath;
    }
}