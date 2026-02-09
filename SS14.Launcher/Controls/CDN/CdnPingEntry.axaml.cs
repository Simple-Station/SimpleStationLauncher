using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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
        UrlName.Text = cdnData.CdnData.Uri.ToString();
        var result = cdnData.Ping;

        if (result.TimeoutMs is not null)
        {
            Status.Text = $"{result.TimeoutMs}ms";
        }
        else
        {
            Status.Text = result.Reason;
        }

        if (result.Error)
        {
            Status.Foreground = Brushes.White;
            Status.Background = Brushes.DarkRed;
        }
    }
}

