using System.Collections.Generic;
using Avalonia.Controls;
using DynamicData;
using SS14.Launcher.Models.CDN;

namespace SS14.Launcher.Controls.CDN;

public partial class CdnPingWindow : Window
{
    public Dictionary<string, CdnPingGroupControl> ActiveCdnGroups = [];

    public CdnPingWindow()
    {
        InitializeComponent();
    }

    public void ResolveItem(CdnDataCompound cdnDataCompound)
    {
        if(!ActiveCdnGroups.TryGetValue(cdnDataCompound.CdnData.Id, out var group))
        {
            group = new CdnPingGroupControl();
            group.GroupLabel.Content = cdnDataCompound.CdnData.Id.ToString();
            ActiveCdnGroups.Add(cdnDataCompound.CdnData.Id, group);
            CdnPanel.Children.Add(group);
        }

        var entry = group.EnsureEntry(cdnDataCompound.CdnData.Uri.AbsoluteUri);
        entry.SetData(cdnDataCompound);
    }
}
