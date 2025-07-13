using System;
using CommunityToolkit.Mvvm.ComponentModel;
using GetYaTube.Models;

namespace GetYaTube.ViewModels;

public partial class DownloadQueueItem : ObservableObject
{
    // Store the video info we got from the YouTube service
    public required VideoDownloadInfo VideoInfo { get; init; }

    [ObservableProperty]
    private string _title;
    
    [ObservableProperty]
    private string _status;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressDisplay))]
    private double _progress;
    
    // A computed property for a nice display format (e.g., "75%")
    public string ProgressDisplay => $"{Progress:P0}";
    
    [ObservableProperty]
    private bool _isIndeterminate; // For when we're busy but don't have progress yet
}