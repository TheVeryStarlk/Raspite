using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Raspite.Library;
using Raspite.WinUI.Messages;
using System.Collections.ObjectModel;
using System.Linq;

namespace Raspite.WinUI.ViewModels;

internal sealed partial class TreeViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Models.File> files = new ObservableCollection<Models.File>();

    [ObservableProperty]
    private int selectedIndex;

    public TreeViewModel()
    {
        WeakReferenceMessenger.Default.Register<FileOpenMessage>(this, FileOpenHandler);
        WeakReferenceMessenger.Default.Register<FileSaveMessage>(this, FileSaveHandler);
    }

    private void FileOpenHandler(object recipient, FileOpenMessage message)
    {
        if (Files.Any(file => file.Name == message.File.Name))
        {
            return;
        }

        Files.Add(message.File);
    }

    private async void FileSaveHandler(object recipient, FileSaveMessage message)
    {
        var file = files.ElementAtOrDefault(selectedIndex);

        if (file is null)
        {
            return;
        }

        // Skip the compound tag we created in the menu view-model.
        NbtTag? parent = file.Node.Tag is NbtTag.Compound compound ? compound.Children.FirstOrDefault() : file.Node.Tag;

        if (parent is null)
        {
            return;
        }

        var bytes = await NbtSerializer.DeserializeAsync(parent, file.Options);
        await System.IO.File.WriteAllBytesAsync(file.Path, bytes);
    }

    [RelayCommand]
    private void OpenFile()
    {
        WeakReferenceMessenger.Default.Send<FileRequestMessage>();
    }
}