using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SS14.Launcher.Models.CDN;

namespace SS14.Launcher.Controls.CDN;

public partial class CdnPingEntry : UserControl
{
    public CdnPingEntry()
    {
        InitializeComponent();
    }

    public void SetData(CdnDataCompound cdnData)
    {
        UrlName.Content = cdnData.CdnData.Uri;
        var result = cdnData.Ping;

        if (result.TimeoutMs is not null)
        {
            Status.Content = $"ping timeout: {result.TimeoutMs}ms";
        }
        else
        {
            Status.Content = result.Reason;
        }
    }
}

