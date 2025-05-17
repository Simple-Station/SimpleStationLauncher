using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using Splat;
using SS14.Launcher.Models.Data;
using SS14.Launcher.Utility;
using SS14.Launcher.ViewModels.MainWindowTabs;

namespace SS14.Launcher.Views.MainWindowTabs;

public partial class OptionsTabView : UserControl
{
    public DataManager Cfg { get; }
    private OptionsTabViewModel Data => (OptionsTabViewModel) DataContext!;

    public OptionsTabView()
    {
        Cfg = Locator.Current.GetRequiredService<DataManager>();
        InitializeComponent();

        Flip.Command = ReactiveCommand.Create(() =>
        {
            var window = (Window?) VisualRoot;
            if (window == null)
                return;

            window.Classes.Add("DoAFlip");

            DispatcherTimer.RunOnce(() => { window.Classes.Remove("DoAFlip"); }, TimeSpan.FromSeconds(1));
        });
    }

    public void ApplyUiScaling(object? sender, RoutedEventArgs args)
    {
        Cfg.SetCVar(CVars.UiScalingX, Data.UiScalingX);
        Cfg.SetCVar(CVars.UiScalingY, Data.UiScalingY);
        Cfg.CommitConfig();
    }

    public async void ClearEnginesPressed(object? _1, RoutedEventArgs _2)
    {
        Data.ClearEngines();
        await ClearEnginesButton.DisplayDoneMessage();
    }

    public async void ClearServerContentPressed(object? _1, RoutedEventArgs _2)
    {
        Data.ClearServerContent();
        await ClearServerContentButton.DisplayDoneMessage();
    }

    private async void OpenHubSettings(object? sender, RoutedEventArgs args)
    {
        await new HubSettingsDialog().ShowDialog((Window)this.GetVisualRoot()!);
    }
}
