using System.Collections.Generic;
using System.Threading.Tasks;
using GetYaTube.Models;

namespace GetYaTube.Services;

public interface IYouTubeService
{
    /// <summary>
    /// Gets a single video's metadata
    /// </summary>
    /// <param name="url">link to youtube video</param>
    /// <returns>Information of the source</returns>
    Task<VideoDownloadInfo> GetVideoInfoAsync(string url);
    
    IAsyncEnumerable<VideoDownloadInfo> GetPlaylistInfoAsync(string url);
}