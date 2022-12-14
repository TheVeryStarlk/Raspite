using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Raspite.WinUI;

internal sealed partial class MenuViewModel : ObservableObject
{
    [RelayCommand]
    private void Exit()
    {
        App.Current.Exit();
    }
}
