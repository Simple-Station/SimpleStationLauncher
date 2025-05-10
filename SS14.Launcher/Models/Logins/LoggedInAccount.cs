using System;
using ReactiveUI;
using SS14.Launcher.Models.Data;

namespace SS14.Launcher.Models.Logins;

public abstract class LoggedInAccount(LoginInfo loginInfo) : ReactiveObject
{
    public LoginInfo LoginInfo { get; } = loginInfo;

    public string Server => LoginInfo.Server;
    public string? ServerUrl => LoginInfo.ServerUrl;
    public string Username => LoginInfo.Username;
    public Guid UserId => LoginInfo.UserId;

    public abstract AccountLoginStatus Status { get; }
}
