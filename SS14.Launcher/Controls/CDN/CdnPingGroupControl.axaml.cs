using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SS14.Launcher.Controls.CDN;

public partial class CdnPingGroupControl : Border
{
    private Dictionary<string, CdnPingEntry> _entries = new();

    public CdnPingGroupControl()
    {
        InitializeComponent();
    }

    public CdnPingEntry EnsureEntry(string key)
    {
        if (_entries.TryGetValue(key, out var entry))
            return entry;

        entry = new CdnPingEntry();
        _entries.Add(key, entry);
        PingInfoContainer.Children.Add(entry);
        return entry;
    }
}

