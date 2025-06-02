using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using DynamicData.Binding;
using Serilog;
using Splat;
using SS14.Launcher.Models.Logins;
using SS14.Launcher.Utility;
using SS14.Launcher.ViewModels;

namespace SS14.Launcher.Views;

public partial class SelectAccountDialog : Window
{
    private readonly SelectAccountDialogViewModel _viewModel;
    public LoggedInAccount? SelectedAccount { get; set; }

    public SelectAccountDialog(string[] authMethods, LoginManager loginManager)
    {
        InitializeComponent();

        _viewModel = (DataContext as SelectAccountDialogViewModel)!;
        _viewModel.Accounts = loginManager.Logins.KeyValues
            .Where(x => authMethods.Contains(x.Value.Server))
            .Select(x => x.Value);
        var viewModelAccounts = _viewModel.Accounts.ToList();
        _viewModel.Error = !viewModelAccounts.Any();
        Log.Error(string.Join(',', viewModelAccounts.Select(x => x.LoginInfo.DisplayName)));
    }

    public void Confirm(object account)
    {
        SelectedAccount = account as LoggedInAccount;
        Close();
    }
}

public sealed class SelectAccountDialogViewModel : IViewModelBase
{
    public IEnumerable<LoggedInAccount> Accounts { get; set; } = new ObservableCollection<LoggedInAccount>();
    public bool Error { get; set; }
}
