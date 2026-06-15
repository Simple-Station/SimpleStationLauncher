using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using SS14.Launcher.Localization;

namespace SS14.Launcher.ViewModels.Login;

public abstract class BaseLoginViewModel : ViewModelBase, IErrorOverlayOwner
{
    [Reactive] public bool Busy { get; protected set; }
    [Reactive] public string? BusyText { get; protected set; }
    [Reactive] public ViewModelBase? OverlayControl { get; set; }
    public MainWindowLoginViewModel ParentVM { get; }

    [Reactive] public string Server { get; set; } = ConfigConstants.AuthUrls.Keys.Select(k => LocalizationManager.Instance.GetString($"login-login-auth-{k}")).First();
    public string? ServerID => Delocalizer.TryGetValue(Server, out var id) ? id : null;
    [Reactive] public List<string> Servers { get; set; } = ConfigConstants.AuthUrls.Keys.Select(k => LocalizationManager.Instance.GetString($"login-login-auth-{k}")).ToList();
    public Dictionary<string, string> Delocalizer = ConfigConstants.AuthUrls.ToDictionary(kv => LocalizationManager.Instance.GetString($"login-login-auth-{kv.Key}"), kv => kv.Key);

    protected BaseLoginViewModel(MainWindowLoginViewModel parentVM)
    {
        ParentVM = parentVM;
    }

    public virtual void Activated()
    {

    }

    public virtual void OverlayOk()
    {
        OverlayControl = null;
    }
}
