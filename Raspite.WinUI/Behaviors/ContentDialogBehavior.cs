using CommunityToolkit.WinUI.UI.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace Raspite.WinUI.Behaviors;

/// <summary>
/// Makes <see cref="ContentDialog"/>'s shadow display over the custom title-bar.
/// </summary>
internal sealed class ContentDialogBehavior : BehaviorBase<FrameworkElement>
{
    /// <inheritdoc/>
    protected override void OnAssociatedObjectLoaded()
    {
        var parent = VisualTreeHelper.GetParent(AssociatedObject);
        var child = VisualTreeHelper.GetChild(parent, 0);
        var smokeLayerBackground = (Rectangle) child;

        smokeLayerBackground.Margin = new Thickness(0);
        smokeLayerBackground.RegisterPropertyChangedCallback(FrameworkElement.MarginProperty, OnMarginChanged);
    }

    private static void OnMarginChanged(DependencyObject sender, DependencyProperty property)
    {
        if (property == FrameworkElement.MarginProperty)
        {
            sender.ClearValue(property);
        }
    }
}