using CommunityToolkit.Mvvm.ComponentModel;

namespace Raspite.WinUI.ViewModels;

internal sealed partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private MenuViewModel menuViewModel;

    public ShellViewModel(MenuViewModel menuViewModel)
    {
        this.menuViewModel = menuViewModel;
    }
}