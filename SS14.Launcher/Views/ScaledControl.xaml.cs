using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Splat;
using SS14.Launcher.Models.Data;
using SS14.Launcher.Utility;

namespace SS14.Launcher.Views;

public sealed partial class ScaledControl : LayoutTransformControl
{
    private readonly DataManager _cfg;


    public ScaledControl()
    {
        _cfg = Locator.Current.GetRequiredService<DataManager>();

        _cfg.GetCVarEntry(CVars.UiScalingX).PropertyChanged += (_, _) =>  RefreshScale();
        _cfg.GetCVarEntry(CVars.UiScalingY).PropertyChanged += (_, _) => RefreshScale();
        _cfg.GetCVarEntry(CVars.UiScalingLock).PropertyChanged += (_, _) => RefreshScale();
    }


    private bool _initialized;
    // Yes, this fucking sucks, I spent so long looking for any event that would fire after the UI exists but couldn't find any.
    // So here we are, with an absurd event I shouldn't be using. Please improve this if you know better.
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_initialized)
        {
            _initialized = true;
            RefreshScale();
        }
    }

    private void RefreshScale() =>
        UpdateScale(_cfg.GetCVar(CVars.UiScalingX), _cfg.GetCVar(CVars.UiScalingY), _cfg.GetCVar(CVars.UiScalingLock));

    public void UpdateScale(double x, double y, bool yx)
    {
        // Get the main window
        if (VisualRoot is not Window window)
            return;

        // Shrink UI to a normal size temporarily
        if (LayoutTransform is ScaleTransform old)
        {
            window.Width /= old.ScaleX;
            window.Height /= old.ScaleY;
        }

        // Apply new scale
        LayoutTransform = new ScaleTransform(x, yx ? x : y);
        window.Width *= x;
        window.Height *= yx ? x : y;
    }
}
