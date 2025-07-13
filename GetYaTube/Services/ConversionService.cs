using System;
using System.IO;
using System.Threading.Tasks;
using GetYaTube.Models;
using Xabe.FFmpeg;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace GetYaTube.Services;

public class ConversionService : IConversionService
{
    private readonly YoutubeClient _youtubeClient = new();

    public ConversionService()
    {
        FFmpeg.SetExecutablesPath(AppContext.BaseDirectory);
    }

    public async Task ConvertAsync(VideoDownloadInfo videoInfo, string outputPath, AudioFormat format,
        IProgress<double> progress)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoInfo.VideoId);
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        
        var outputExtension = format == AudioFormat.Mp3 ? ".mp3" : ".flac";
        var finalFilePath = Path.ChangeExtension(outputPath, outputExtension);

        var conversion =
            (IConversion)await FFmpeg.Conversions.FromSnippet.ExtractAudio(audioStreamInfo.Url, finalFilePath);

        if (format == AudioFormat.Mp3)
        {
            conversion.SetAudioBitrate(192);
        }

        conversion.OnProgress += (sender, args) =>
        {
            progress.Report(args.Percent / 100.0);
        };

        await conversion.Start();
    }
}