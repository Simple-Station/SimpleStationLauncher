using System;
using System.Diagnostics;
using Splat;
using SS14.Launcher.Localization;
using SS14.Launcher.Models.ContentManagement;
using SS14.Launcher.Models.Data;
using SS14.Launcher.Models.EngineManager;
using SS14.Launcher.Models.Logins;
using SS14.Launcher.Utility;

namespace SS14.Launcher.ViewModels.MainWindowTabs;

public class OptionsTabViewModel : MainWindowTabViewModel
{
    public DataManager Cfg { get; }
    private readonly IEngineManager _engineManager;
    private readonly ContentManager _contentManager;
    private readonly LoginManager _loginMgr;

    public LanguageSelectorViewModel Language { get; } = new();

    public OptionsTabViewModel()
    {
        Cfg = Locator.Current.GetRequiredService<DataManager>();
        _loginMgr = Locator.Current.GetRequiredService<LoginManager>();
        _engineManager = Locator.Current.GetRequiredService<IEngineManager>();
        _contentManager = Locator.Current.GetRequiredService<ContentManager>();

        DisableIncompatibleMacOS = OperatingSystem.IsMacOS();
        _uiScalingX = Cfg.GetCVar(CVars.UiScalingX);
        _uiScalingY = Cfg.GetCVar(CVars.UiScalingY);
    }
    public bool DisableIncompatibleMacOS { get; }

#if RELEASE
    public bool HideDisableSigning => true;
#else
    public bool HideDisableSigning => false;
#endif

    public override string Name => LocalizationManager.Instance.GetString("tab-options-title");

    public bool CompatMode
    {
        get => Cfg.GetCVar(CVars.CompatMode);
        set
        {
            Cfg.SetCVar(CVars.CompatMode, value);
            Cfg.CommitConfig();
        }
    }

    public bool LogClient
    {
        get => Cfg.GetCVar(CVars.LogClient);
        set
        {
            Cfg.SetCVar(CVars.LogClient, value);
            Cfg.CommitConfig();
        }
    }

    public bool LogLauncher
    {
        get => Cfg.GetCVar(CVars.LogLauncher);
        set
        {
            Cfg.SetCVar(CVars.LogLauncher, value);
            Cfg.CommitConfig();
        }
    }

    public bool LogLauncherVerbose
    {
        get => Cfg.GetCVar(CVars.LogLauncherVerbose);
        set
        {
            Cfg.SetCVar(CVars.LogLauncherVerbose, value);
            Cfg.CommitConfig();
        }
    }

    public bool DisableSigning
    {
        get => Cfg.GetCVar(CVars.DisableSigning);
        set
        {
            Cfg.SetCVar(CVars.DisableSigning, value);
            Cfg.CommitConfig();
        }
    }

    private double _uiScalingX;
    public double UiScalingX
    {
        get => _uiScalingX;
        set => _uiScalingX = Math.Clamp(value, 0.1, 10);
    }

    private double _uiScalingY;
    public double UiScalingY
    {
        get => _uiScalingY;
        set => _uiScalingY = Math.Clamp(value, 0.1, 10);
    }

    public bool NotUiScalingLock => !UiScalingLock;
    public bool UiScalingLock
    {
        get => Cfg.GetCVar(CVars.UiScalingLock);
        set
        {
            Cfg.SetCVar(CVars.UiScalingLock, value);
            Cfg.CommitConfig();
        }
    }

    public bool OverrideAssets
    {
        get => Cfg.GetCVar(CVars.OverrideAssets);
        set
        {
            Cfg.SetCVar(CVars.OverrideAssets, value);
            Cfg.CommitConfig();
        }
    }

    public void ClearEngines()
    {
        _engineManager.ClearAllEngines();
    }

    public void ClearServerContent()
    {
        _contentManager.ClearAll();
    }

    public void OpenLogDirectory()
    {
        Process.Start(new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = LauncherPaths.DirLogs
        });
    }

    public void OpenAccountSettings()
    {
        if (_loginMgr.ActiveAccount is not { } account
            || LoginManager.TryGetAccountUrl(account.Server, account.ServerUrl) is not { } url)
            return;
        Helpers.OpenUri(LoginManager.GetAuthServerById(account.Server, account.ServerUrl, url).AccountManUrl);
    }
}
