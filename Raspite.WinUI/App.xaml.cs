using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace Raspite.WinUI;

public sealed partial class App : Application
{
    public new static App Current => (App) Application.Current;

    public IServiceProvider Services { get; }

    private Window? window;

    public App()
    {
        Services = new ServiceCollection()
            .AddTransient<ShellView>()
            .AddTransient<ShellViewModel>()
            .BuildServiceProvider();

        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs eventArgs)
    {
        window = Services.GetRequiredService<ShellView>();
        window.ExtendsContentIntoTitleBar = true;
        window.Activate();
    }
}