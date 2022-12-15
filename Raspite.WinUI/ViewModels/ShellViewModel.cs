using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Raspite.WinUI.Messages;
using System;

namespace Raspite.WinUI.ViewModels;

internal sealed partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private MenuViewModel menuViewModel;

    [ObservableProperty]
    private bool isOpen;

    [ObservableProperty]
    private string? message;

    public ShellViewModel(MenuViewModel menuViewModel)
    {
        this.menuViewModel = menuViewModel;
        WeakReferenceMessenger.Default.Register<NotificationRequestMessage>(this, NotificationRequestHandler);
    }

    private void NotificationRequestHandler(object recipient, NotificationRequestMessage message)
    {
        IsOpen = true;
        Message = message.Message;
    }
}