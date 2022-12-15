using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Raspite.WinUI.Messages;
using Raspite.WinUI.Models;
using Raspite.WinUI.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Raspite.WinUI.ViewModels;

internal sealed partial class MenuViewModel : ObservableObject
{
    private readonly DialogService dialogService;
    private readonly NbtSerializerService nbtSerializerService;

    public MenuViewModel(DialogService dialogService, NbtSerializerService nbtSerializerService)
    {
        this.dialogService = dialogService;
        this.nbtSerializerService = nbtSerializerService;

        WeakReferenceMessenger.Default.Register<FileRequestMessage>(this,
            async (_, _) => await OpenFileAsync());
    }

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var path = await dialogService.ShowFileDialogAsync();

        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var tag = await nbtSerializerService.SerializeAsync(await System.IO.File.ReadAllBytesAsync(path));

        if (tag is null)
        {
            WeakReferenceMessenger.Default.Send(new NotificationRequestMessage("An error has occured while trying to serialize the file."));
            return;
        }

        WeakReferenceMessenger.Default.Send(new FileOpenMessage(new Models.File(path, tag)));
    }

    [RelayCommand]
    private async Task AboutAsync()
    {
        await dialogService.ShowMessageDialogAsync(
            "An application that helps with editing NBT files.",
            "About");
    }

    [RelayCommand]
    private void Exit()
    {
        App.Current.Exit();
    }
}
