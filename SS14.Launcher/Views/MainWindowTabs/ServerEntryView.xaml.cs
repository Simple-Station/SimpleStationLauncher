using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.LogicalTree;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Splat;
using SS14.Launcher.Models.Data;
using SS14.Launcher.Utility;
using SS14.Launcher.ViewModels.MainWindowTabs;

namespace SS14.Launcher.Views.MainWindowTabs;

public partial class ServerEntryView : UserControl
{
    private readonly DataManager _cfg;


    public ServerEntryView()
    {
        InitializeComponent();

        _cfg = Locator.Current.GetRequiredService<DataManager>();

        Links.LayoutUpdated += ApplyStyle;
        Expando.Expanded += (_, _) =>  UpdateExpanse();
        Expando.Collapsed += (_, _) => UpdateExpanse();
    }


    // Sets the style for the link buttons correctly so that they look correct
    private void ApplyStyle(object? _1, EventArgs _2)
    {
        for (var i = 0; i < Links.ItemCount; i++)
        {
            if (Links.ContainerFromIndex(i) is not ContentPresenter { Child: ServerInfoLinkControl control } presenter)
                continue;

            presenter.ApplyTemplate();

            if (Links.ItemCount == 1)
                return;

            var style = i switch
            {
                0 => "OpenRight",
                _ when i == Links.ItemCount - 1 => "OpenLeft",
                _ => "OpenBoth",
            };

            control.GetLogicalChildren().OfType<Button>().FirstOrDefault()?.Classes.Add(style);
        }
    }

    private void UpdateExpanse()
    {
        if (DataContext is not ServerEntryViewModel vm
            || vm.Favorite == null)
            return;

        if (Expando.IsExpanded)
            _cfg.ExpandedServers.Add(vm.Favorite.Address);
        else
            _cfg.ExpandedServers.Remove(vm.Favorite.Address);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (DataContext is ObservableRecipient r)
            r.IsActive = true;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (DataContext is ObservableRecipient r)
            r.IsActive = false;
    }
}
