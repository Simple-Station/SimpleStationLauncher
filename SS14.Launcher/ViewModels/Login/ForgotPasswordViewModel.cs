using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SS14.Launcher.Api;
using SS14.Launcher.Localization;
using SS14.Launcher.Models.Logins;

namespace SS14.Launcher.ViewModels.Login;

public sealed class ForgotPasswordViewModel : BaseLoginViewModel
{
    private readonly AuthApi _authApi;
    private readonly LocalizationManager _loc = LocalizationManager.Instance;

    [Reactive] public string? ServerUrl { get; set; }
    [Reactive] public string ServerUrlPlaceholder { get; set; } = ConfigConstants.AuthUrls.First().Value.AuthUrl.ToString();
    [Reactive] public bool IsCustom { get; private set; }
    [Reactive] public bool IsServerPotentiallyValid { get; private set; }

    [Reactive] public string EditingEmail { get; set; } = "";
    [Reactive] public bool IsInputValid { get; private set; }

    private bool _errored;

    public ForgotPasswordViewModel(
        MainWindowLoginViewModel parentVM,
        AuthApi authApi)
        : base(parentVM)
    {
        _authApi = authApi;

        this.WhenAnyValue(x => x.Server, x => x.ServerUrl, x => x.EditingEmail)
            .Subscribe(s =>
            {
                IsCustom = ServerID == ConfigConstants.CustomAuthServer;
                IsInputValid = IsCustom
                    ? !string.IsNullOrEmpty(s.Item2) && !string.IsNullOrEmpty(s.Item3)
                    : !string.IsNullOrEmpty(ServerID) && !string.IsNullOrEmpty(s.Item3);
                ServerUrlPlaceholder = IsCustom ? ServerUrl : LoginManager.GetAuthServerById(ServerID).AuthUrl.ToString();
                IsServerPotentiallyValid = !IsCustom || !Busy && Uri.TryCreate(ServerUrl, UriKind.Absolute, out _);
            });
    }

    public async void SubmitPressed()
    {
        if (Busy)
            return;

        Busy = true;
        try
        {
            BusyText = "Sending email...";
            var errors = await _authApi.ForgotPasswordAsync(Server, ServerUrl, EditingEmail);

            _errored = errors != null;

            if (!_errored)
            {
                // This isn't an error lol but that's what I called the control.
                OverlayControl = new AuthErrorsOverlayViewModel(this, _loc.GetString("login-forgot-success-title"), new[]
                {
                    _loc.GetString("login-forgot-success-message")
                });
            }
            else
            {
                OverlayControl = new AuthErrorsOverlayViewModel(this, _loc.GetString("login-forgot-error"), errors!);
            }
        }
        finally
        {
            Busy = false;
        }
    }

    public override void OverlayOk()
    {
        if (_errored)
        {
            base.OverlayOk();
        }
        else
        {
            // If the overlay was a success overlay, switch back to login.
            ParentVM.SwitchToLogin();
        }
    }
}
