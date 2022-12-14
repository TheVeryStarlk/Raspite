using Microsoft.Extensions.DependencyInjection;
using WinUIEx;

namespace Raspite.WinUI;

public sealed partial class ShellView : WindowEx
{
    public ShellView()
    {
        InitializeComponent();
        SetTitleBar(AppTitleBar);

        Root.DataContext = App.Current.Services.GetRequiredService<ShellViewModel>();
    }
}