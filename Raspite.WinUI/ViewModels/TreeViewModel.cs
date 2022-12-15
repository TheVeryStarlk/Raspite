using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Raspite.WinUI.Messages;
using Raspite.WinUI.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Raspite.WinUI.ViewModels;

internal sealed partial class TreeViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<File> files = new ObservableCollection<File>();

    public TreeViewModel()
    {
        WeakReferenceMessenger.Default.Register<FileOpenMessage>(this, FileOpenHandler);
    }

    private void FileOpenHandler(object sender, FileOpenMessage message)
    {
        if (Files.Any(file => file.Name == message.File.Name))
        {
            return;
        }

        Files.Add(message.File);
    }

    [RelayCommand]
    private void OpenFile()
    {
        WeakReferenceMessenger.Default.Send<FileRequestMessage>();
    }
}