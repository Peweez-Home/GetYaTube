using System;
using System.Threading.Tasks;
using GetYaTube.Models;

namespace GetYaTube.Services;


public enum AudioFormat { Mp3, Flac }

public interface IConversionService
{
    Task ConvertAsync(VideoDownloadInfo videoInfo, string outputPath, AudioFormat format, IProgress<double> progress);
}