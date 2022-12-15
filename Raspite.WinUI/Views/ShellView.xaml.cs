using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Raspite.WinUI.ViewModels;
using System.ComponentModel;
using WinUIEx;

namespace Raspite.WinUI.Views;

public sealed partial class ShellView : WindowEx
{
    private readonly ShellViewModel viewModel;

    public ShellView()
    {
        InitializeComponent();
        SetTitleBar(AppTitleBar);

        viewModel = App.Current.Services!.GetRequiredService<ShellViewModel>();
        Root.DataContext = viewModel;

        viewModel.PropertyChanged += ViewModelPropertyChanged;
    }

    private void ViewModelPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(viewModel.IsOpen))
        {
            if (viewModel.IsOpen)
            {
                Notification.Visibility = Visibility.Visible;
                Notification.Opacity = 1;
            }
            else
            {
                Notification.Visibility = Visibility.Collapsed;
                Notification.Opacity = 0;
            }
        }
    }

    private async void OpenShortcutInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs eventArgs)
    {
        await viewModel.MenuViewModel.OpenFileCommand.ExecuteAsync(null);
    }
}