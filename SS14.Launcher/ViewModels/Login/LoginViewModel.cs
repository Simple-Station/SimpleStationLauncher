using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SS14.Launcher.Api;
using SS14.Launcher.Localization;
using SS14.Launcher.Models.Data;
using SS14.Launcher.Models.Logins;
using Serilog;

namespace SS14.Launcher.ViewModels.Login;

public class LoginViewModel : BaseLoginViewModel
{
    private readonly AuthApi _authApi;
    private readonly LoginManager _loginMgr;
    private readonly DataManager _dataManager;
    private readonly LocalizationManager _loc = LocalizationManager.Instance;

    [Reactive] public string Server { get; set; } = ConfigConstants.AuthUrls.First().Key;
    [Reactive] public List<string> Servers { get; set; } = ConfigConstants.AuthUrls.Keys.ToList();
    [Reactive] public string? ServerUrl { get; set; }
    [Reactive] public string ServerUrlPlaceholder { get; set; } = ConfigConstants.AuthUrls.First().Value.AuthUrl.ToString();
    [Reactive] public bool IsCustom { get; private set; }
    [Reactive] public bool IsServerPotentiallyValid { get; private set; }

    [Reactive] public string EditingUsername { get; set; } = "";
    [Reactive] public string EditingPassword { get; set; } = "";

    [Reactive] public bool IsInputValid { get; private set; }
    [Reactive] public bool IsPasswordVisible { get; set; }

    public LoginViewModel(MainWindowLoginViewModel parentVm, AuthApi authApi,
        LoginManager loginMgr, DataManager dataManager) : base(parentVm)
    {
        BusyText = _loc.GetString("login-login-busy-logging-in");
        _authApi = authApi;
        _loginMgr = loginMgr;
        _dataManager = dataManager;

        this.WhenAnyValue(x => x.Server, x => x.ServerUrl, x => x.EditingUsername, x => x.EditingPassword)
            .Subscribe(s =>
            {
                IsInputValid = s.Item1 == ConfigConstants.CustomAuthServer
                    ? !string.IsNullOrEmpty(s.Item2) && !string.IsNullOrEmpty(s.Item2) && !string.IsNullOrEmpty(s.Item3)
                    : !string.IsNullOrEmpty(s.Item1) && !string.IsNullOrEmpty(s.Item3);
                IsCustom = Server == ConfigConstants.CustomAuthServer;
                ServerUrlPlaceholder = LoginManager.GetAuthServerById(IsCustom ? ConfigConstants.AuthUrls.First().Key : Server).AuthUrl.ToString();
                IsServerPotentiallyValid = !IsCustom || !Busy && Uri.TryCreate(ServerUrl, UriKind.Absolute, out _);
            });
    }

    public async void OnLogInButtonPressed()
    {
        if (!IsInputValid || Busy)
        {
            return;
        }

        Busy = true;
        try
        {
            var request = new AuthApi.AuthenticateRequest(Server, ServerUrl, EditingUsername, null, EditingPassword);
            var resp = await _authApi.AuthenticateAsync(request);

            await DoLogin(this, request, resp, _loginMgr, _authApi);

            _dataManager.CommitConfig();
        }
        finally
        {
            Busy = false;
        }
    }

    public static async Task<bool> DoLogin<T>(
        T vm,
        AuthApi.AuthenticateRequest request,
        AuthenticateResult resp,
        LoginManager loginMgr,
        AuthApi authApi)
        where T : BaseLoginViewModel, IErrorOverlayOwner
    {
        var loc = LocalizationManager.Instance;
        if (resp.IsSuccess)
        {
            var loginInfo = resp.LoginInfo;
            loginInfo.ServerUrl ??= ConfigConstants.AuthUrls[ConfigConstants.FallbackAuthServer].AuthUrl.AbsoluteUri;
            Log.Information($"Login successful for user {loginInfo.UserId} on server {loginInfo.Server} at {loginInfo.ServerUrl}");
            var oldLogin = loginMgr.Logins.KeyValues.FirstOrDefault(a =>
                a.Key == loginInfo.UserId && a.Value.Server == loginInfo.Server
                    && a.Value.ServerUrl == loginInfo.ServerUrl).Value;
            if (oldLogin != null)
            {
                // Already had this login, apparently.
                // Thanks user.
                //
                // Log the OLD token out since we don't need two of them.
                // This also has the upside of re-available-ing the account
                // if the user used the main login prompt on an account we already had, but as expired.

                await authApi.LogoutTokenAsync(oldLogin.Server, oldLogin.ServerUrl, oldLogin.LoginInfo.Token.Token);
                loginMgr.ActiveAccountId = loginInfo.UserId;
                loginMgr.UpdateToNewToken(loginMgr.ActiveAccount!, loginInfo.Token);
                return true;
            }

            loginMgr.AddFreshLogin(loginInfo);
            loginMgr.ActiveAccountId = loginInfo.UserId;
            return true;
        }

        if (resp.Code == AuthApi.AuthenticateDenyResponseCode.TfaRequired)
        {
            vm.ParentVM.SwitchToAuthTfa(request);
            return false;
        }

        var errors = AuthErrorsOverlayViewModel.AuthCodeToErrors(resp.Errors, resp.Code);
        vm.OverlayControl = new AuthErrorsOverlayViewModel(vm, loc.GetString("login-login-error-title"), errors);
        return false;
    }

    // Registration is purely via website for now
    public void RegisterPressed() =>
        Helpers.OpenUri(LoginManager.GetAuthServerById(Server, ServerUrl).AccountRegUrl);
    public void ResendConfirmationPressed() =>
        Helpers.OpenUri(LoginManager.GetAuthServerById(Server, ServerUrl).AccountResendUrl);
}
