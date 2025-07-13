using Avalonia.Controls;
using GetYaTube.ViewModels;

namespace GetYaTube.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}