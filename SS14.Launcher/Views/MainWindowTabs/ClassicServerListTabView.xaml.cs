using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SS14.Launcher.Views.MainWindowTabs;

public partial class ClassicServerListTabView : UserControl
{
    public ClassicServerListTabView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
