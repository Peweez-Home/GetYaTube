using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetYaTube.Models;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace GetYaTube.Services;

public class YouTubeService : IYouTubeService
{
    private readonly YoutubeClient _youtubeClient = new();

    public async Task<VideoDownloadInfo> GetVideoInfoAsync(string url)
    {
        var video = await _youtubeClient.Videos.GetAsync(url);
        return MapVideoToDownloadInfo(video);
    }

    public async IAsyncEnumerable<VideoDownloadInfo> GetPlaylistInfoAsync(string url)
    {
        await foreach (var video in _youtubeClient.Playlists.GetVideosAsync(url))
        {
            yield return MapVideoToDownloadInfo(video);
        }
    }

    private static VideoDownloadInfo MapVideoToDownloadInfo(IVideo video)
    {
        return new VideoDownloadInfo
        {
            VideoId = video.Id.Value,
            Title = video.Title,
            Author = video.Author.ChannelTitle,
            Duration = video.Duration ?? TimeSpan.Zero
        };
    }
}