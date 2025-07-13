using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using GetYaTube.Services;
using GetYaTube.ViewModels;
using GetYaTube.Views;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace GetYaTube;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // This now works because the Locator was correctly set up in Program.cs
            desktop.MainWindow = Locator.Current.GetService<MainWindow>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = Locator.Current.GetService<MainView>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}