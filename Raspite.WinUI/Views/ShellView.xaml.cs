using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Input;
using Raspite.WinUI.ViewModels;
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
    }

    private async void OpenShortcutInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs eventArgs)
    {
        await viewModel.MenuViewModel.OpenFileCommand.ExecuteAsync(null);
    }
}