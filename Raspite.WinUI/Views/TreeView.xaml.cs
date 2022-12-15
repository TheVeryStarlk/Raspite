using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Raspite.WinUI.ViewModels;

namespace Raspite.WinUI.Views;

public sealed partial class TreeView : UserControl
{
    public TreeView()
    {
        DataContext = App.Current.Services?.GetRequiredService<TreeViewModel>();
        InitializeComponent();
    }
}
