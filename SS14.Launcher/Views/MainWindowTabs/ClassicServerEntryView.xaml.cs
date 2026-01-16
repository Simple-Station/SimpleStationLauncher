using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SS14.Launcher.Views.MainWindowTabs;

public partial class ClassicServerEntryView : UserControl
{
    public ClassicServerEntryView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
