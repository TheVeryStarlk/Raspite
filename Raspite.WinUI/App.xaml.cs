using Microsoft.UI.Xaml;

namespace Raspite.WinUI;

public sealed partial class App : Application
{
    private Window? window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs eventArgs)
    {
        window = new ShellView()
        {
            ExtendsContentIntoTitleBar = true
        };

        window.Activate();
    }
}