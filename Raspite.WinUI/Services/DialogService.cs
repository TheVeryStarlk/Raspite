using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using WinUIEx;
using Microsoft.Xaml.Interactivity;
using Raspite.WinUI.Behaviors;

namespace Raspite.WinUI.Services;

internal sealed class DialogService
{
    private Window? Window => App.Current.Window;

    public async Task<string?> ShowFileDialogAsync()
    {
        var picker = Window?.CreateOpenFilePicker();
        picker?.FileTypeFilter.Add("*");

        var storage = await picker?.PickSingleFileAsync();
        return storage?.Path;
    }

    public async Task ShowMessageDialogAsync(string message, string title, string button = "Got it!")
    {
        var dialog = new ContentDialog
        {
            XamlRoot = Window?.Content.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = title,
            Content = message,
            PrimaryButtonText = button,
            DefaultButton = ContentDialogButton.Primary
        };

        Interaction.SetBehaviors(dialog, new BehaviorCollection()
        {
            new ContentDialogBehavior()
        });

        await dialog.ShowAsync();
    }
}