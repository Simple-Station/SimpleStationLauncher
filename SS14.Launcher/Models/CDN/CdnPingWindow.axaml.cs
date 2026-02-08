using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;

namespace SS14.Launcher.Models.CDN;

public partial class CdnPingWindow : Window
{
    public Dictionary<string, Label> ActiveCdnPing = [];

    public CdnPingWindow()
    {
        InitializeComponent();
    }

    public void ResolveItem(CdnDataCompound cdnDataCompound)
    {
        ResolveItem(cdnDataCompound.CdnData, cdnDataCompound.Ping);
    }

    public void ResolveItem(UriCdnData data, CdnPingResponse result)
    {
        var label = GetOrAddCdnPingLabel(data.ToString());

        if (result.TimeoutMs is not null)
        {
            label.Content = $"{data.Id} {data.Uri} ping timeout: {result.TimeoutMs}ms";
        }
        else
        {
            label.Content = $"{data.Id} {data.Uri} {result.Reason}";
        }
    }

    private Label GetOrAddCdnPingLabel(string key)
    {
        if (ActiveCdnPing.TryGetValue(key, out var label))
            return label;

        label = new Label();
        ActiveCdnPing.Add(key, label);
        CdnPanel.Children.Add(label);
        return label;
    }
}

public class CdnPingInfoControl : StackPanel
{


    public CdnPingInfoControl()
    {
        Orientation = Avalonia.Layout.Orientation.Horizontal;
        Spacing = 5f;
    }
}
