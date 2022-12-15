using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Raspite.WinUI.Models;
using Raspite.WinUI.ViewModels;
using System.Linq;
using Windows.Foundation.Collections;

namespace Raspite.WinUI.Views;

public sealed partial class TreeView : UserControl
{
    private readonly TreeViewModel? viewModel;

    private bool hasShown;

    public TreeView()
    {
        viewModel = App.Current.Services?.GetRequiredService<TreeViewModel>();
        DataContext = viewModel;

        InitializeComponent();

        Tab.TabItemsChanged += TabViewTabItemsChanged;
    }

    private void TabViewTabItemsChanged(TabView sender, IVectorChangedEventArgs eventArgs)
    {
        if (!hasShown)
        {
            FeelsEmpty.Opacity = 0;
            Tab.SelectedIndex = 0;
            Tab.Opacity = 1;

            hasShown = true;
        }
    }

    private void TabTabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs eventArgs)
    {
        var file = (File) eventArgs.Item;
        viewModel?.Files.Remove(viewModel?.Files.First(predicate => predicate.Path == file.Path)!);

        if (viewModel?.Files.Count == 0)
        {
            hasShown = false;

            FeelsEmpty.Opacity = 1;
            Tab.Opacity = 0;
        }
    }
}