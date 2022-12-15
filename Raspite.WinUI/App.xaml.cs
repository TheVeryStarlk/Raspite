using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace Raspite.WinUI;

public sealed partial class App : Application
{
    public new static App Current => (App) Application.Current;

    public IServiceProvider Services { get; }

    public Window? Window { get; private set; }

    public App()
    {
        Services = new ServiceCollection()
            .AddTransient<ShellViewModel>()
            .AddTransient<MenuViewModel>()
            .AddTransient<DialogService>()
            .AddTransient<NbtSerializerService>()
            .BuildServiceProvider();

        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs eventArgs)
    {
        Window = new ShellView()
        {
            ExtendsContentIntoTitleBar = true
        };

        Window.Activate();
    }
}