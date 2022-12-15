using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Raspite.WinUI;

internal sealed partial class MenuViewModel : ObservableObject
{
    private readonly DialogService dialogService;

    public MenuViewModel(DialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        await dialogService.ShowFileDialogAsync();
    }

    [RelayCommand]
    private void Exit()
    {
        App.Current.Exit();
    }
}
