using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WinUIEx;

namespace Raspite.WinUI;

internal sealed class DialogService
{
    public async Task<string?> ShowFileDialogAsync()
    {
        var picker = App.Current.Window?.CreateOpenFilePicker();
        picker?.FileTypeFilter.Add("*");

        var file = await picker?.PickSingleFileAsync();
        return file?.Path;
    }
}