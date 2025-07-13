using System;

namespace GetYaTube.Models;

public class VideoDownloadInfo
{
    public required string VideoId { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public TimeSpan Duration { get; init; }
}