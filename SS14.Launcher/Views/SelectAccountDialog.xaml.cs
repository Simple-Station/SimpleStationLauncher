using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using SS14.Launcher.Localization;
using SS14.Launcher.Models.Logins;
using SS14.Launcher.Utility;

namespace SS14.Launcher.Views;

public partial class SelectAccountDialog : Window
{
    private readonly LocalizationManager _loc;
    private readonly LoginManager _loginMgr;

    public LoggedInAccount? SelectedAccount { get; set; }
    public IEnumerable<LoggedInAccount> Accounts { get; set; }
    public bool Error { get; set; }
    public string Description { get; set; } = string.Empty;

    public SelectAccountDialog(string[] authMethods, LoginManager loginManager)
    {
        InitializeComponent();

        _loc = LocalizationManager.Instance;
        _loginMgr = loginManager;

        Accounts = _loginMgr.Logins.KeyValues
            .Where(x => authMethods.FirstOrDefault(m => m == ConfigConstants.AuthUrls[x.Value.Server].AuthUrl.AbsoluteUri) != null)
            .Select(x => x.Value);
        Error = !Accounts.Any();
        Description = _loc.GetString("select-account-dialog-description", ("allowedAuths", string.Join(", ", authMethods.Select(m => ConfigConstants.AuthUrls.FirstOrDefault(kv => kv.Value.AuthUrl.AbsoluteUri == m).Key))));
    }

    public void Confirm(object account)
    {
        SelectedAccount = account as LoggedInAccount;
        Close();
    }

    public void AddAccount() {
        _loginMgr.ActiveAccount = null;
        Close();
    }
}
