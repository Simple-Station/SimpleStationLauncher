using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using SS14.Launcher.Models.Logins;
using SS14.Launcher.Utility;

namespace SS14.Launcher.Views;

public partial class SelectAccountDialog : Window
{
    public LoggedInAccount? SelectedAccount { get; set; }
    public IEnumerable<LoggedInAccount> Accounts { get; set; }
    public bool Error { get; set; }

    public SelectAccountDialog(string[] authMethods, LoginManager loginManager)
    {
        InitializeComponent();

        Accounts = loginManager.Logins.KeyValues
            .Where(x => authMethods.Contains(x.Value.Server))
            .Select(x => x.Value);
        Error = !Accounts.Any();
    }

    public void Confirm(object account)
    {
        SelectedAccount = account as LoggedInAccount;
        Close();
    }
}
