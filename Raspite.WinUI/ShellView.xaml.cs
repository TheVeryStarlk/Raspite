using WinUIEx;

namespace Raspite.WinUI;

public sealed partial class ShellView : WindowEx
{
    public ShellView()
    {
        InitializeComponent();
        SetTitleBar(Bar);
    }
}