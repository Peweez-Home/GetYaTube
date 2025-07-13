// GetYaTube.Desktop/Program.cs
using Avalonia;
using System;
using GetYaTube.Services;
using GetYaTube.ViewModels;
using GetYaTube.Views;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace GetYaTube.Desktop;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // 1. Create a service collection
        var services = new ServiceCollection();
        
        // 2. Register all our services and viewmodels
        ConfigureServices(services);
        
        // 3. THIS IS THE MAGIC LINE.
        // It configures Splat to use the Microsoft DI container,
        // handling the entire initialization lifecycle correctly.
        // It MUST be called before building the App.
        services.UseMicrosoftDependencyResolver();
        
        // 4. Now, build and run the app as normal.
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .WithInterFont();

    private static void ConfigureServices(IServiceCollection services)
    {
        // Register Platform-Specific Services
        services.AddSingleton<IPathService, DesktopPathService>();

        // Register Shared Services
        services.AddSingleton<IYouTubeService, YouTubeService>();
        services.AddSingleton<IConversionService, ConversionService>();

        // Register ViewModels
        services.AddTransient<MainViewModel>();
        
        // Register Views/Windows that need DI
        services.AddTransient<MainWindow>();
        services.AddTransient<MainView>();
    }
}