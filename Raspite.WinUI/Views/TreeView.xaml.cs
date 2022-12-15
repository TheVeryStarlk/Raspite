using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Raspite.WinUI.ViewModels;
using Windows.Foundation.Collections;

namespace Raspite.WinUI.Views;

public sealed partial class TreeView : UserControl
{
    private bool hasShown;

    public TreeView()
    {
        DataContext = App.Current.Services?.GetRequiredService<TreeViewModel>();
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
}