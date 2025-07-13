using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetYaTube.Services;

namespace GetYaTube.ViewModels;


public partial class MainViewModel : ViewModelBase
{
    private readonly IYouTubeService _youTubeService;
    private readonly IConversionService _conversionService;
    private readonly IPathService _pathService;

    // --- Bindable Properties ---
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ProcessUrlCommand))]
    private string _url = string.Empty;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private string _statusMessage = "Ready";

    public ObservableCollection<DownloadQueueItem> DownloadQueue { get; } = new();

    public MainViewModel(IYouTubeService youTubeService, IConversionService conversionService, IPathService pathService)
    {
        _youTubeService = youTubeService;
        _conversionService = conversionService;
        _pathService = pathService;
    }

    private bool CanProcessUrl() => !string.IsNullOrWhiteSpace(Url) && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanProcessUrl))]
    private async Task ProcessUrlAsync()
    {
        IsBusy = true;
        StatusMessage = "Fetching video info...";
        DownloadQueue.Clear();

        try
        {
            var itemsToProcess = new List<DownloadQueueItem>();

            // Check if it's a playlist or a single video
            if (Url.Contains("playlist?list=", StringComparison.OrdinalIgnoreCase))
            {
                StatusMessage = "Fetching playlist videos...";
                // Asynchronously stream playlist info without blocking the UI
                await foreach (var videoInfo in _youTubeService.GetPlaylistInfoAsync(Url))
                {
                    var queueItem = new DownloadQueueItem
                    {
                        VideoInfo = videoInfo, // Store the full info object
                        Title = videoInfo.Title,
                        Status = "Queued",
                        IsIndeterminate = true
                    };
                    DownloadQueue.Add(queueItem);
                    itemsToProcess.Add(queueItem);
                }
            }
            else // It's a single video
            {
                var videoInfo = await _youTubeService.GetVideoInfoAsync(Url);
                var queueItem = new DownloadQueueItem
                {
                    VideoInfo = videoInfo,
                    Title = videoInfo.Title,
                    Status = "Queued",
                    IsIndeterminate = true
                };
                DownloadQueue.Add(queueItem);
                itemsToProcess.Add(queueItem);
            }

            StatusMessage = $"Processing {itemsToProcess.Count} item(s)...";

            // Now, process each item in the queue one by one.
            // For a more advanced app, you could run them in parallel.
            foreach (var item in itemsToProcess)
            {
                await ProcessQueueItemAsync(item);
            }

            StatusMessage = "All items processed.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private async Task ProcessQueueItemAsync(DownloadQueueItem item)
    {
        item.IsIndeterminate = false;
        item.Status = "Downloading & Converting...";
        item.Progress = 0;

        try
        {
            // Get the default download path from our platform-specific service
            var downloadFolder = _pathService.GetDefaultDownloadPath();
        
            // Ensure the directory exists
            Directory.CreateDirectory(downloadFolder);

            // Sanitize the video title to create a valid file name
            var sanitizedTitle = string.Join("_", item.VideoInfo.Title.Split(Path.GetInvalidFileNameChars()));
            var outputPath = Path.Combine(downloadFolder, sanitizedTitle);
        
            // Create a progress reporter that updates the item's Progress property
            var progressHandler = new Progress<double>(p => item.Progress = p);

            // TODO: Let the user choose the format. For now, we'll hardcode Mp3.
            var format = AudioFormat.Mp3;

            // Call the conversion service!
            await _conversionService.ConvertAsync(item.VideoInfo, outputPath, format, progressHandler);

            // If we get here, it succeeded
            item.Progress = 1.0;
            item.Status = "Completed";
        }
        catch (Exception ex)
        {
            // Handle any errors during the conversion
            item.Progress = 0;
            item.Status = $"Error: {ex.Message}";
        }
    }
}