using System;
using System.Diagnostics;
using Microsoft.Win32;
using ReactiveUI;
using Serilog;
using Splat;
using SS14.Launcher.Localization;
using SS14.Launcher.Models;
using SS14.Launcher.Models.ServerStatus;
using SS14.Launcher.Utility;

namespace SS14.Launcher.ViewModels.MainWindowTabs;

public class ClassicServerEntryViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindow;
    private readonly ClassicServerStatusData _server;

    public string Name => _server.Name;
    public string Address => _server.Address;
    public string PlayerCount => _server.PlayerCount.ToString();
    public string Status => _server.Status;
    public string RoundTime => _server.RoundTime;

    private bool _isExpanded;

    public bool IsExpanded
    {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> ConnectCommand { get; }

    public ClassicServerEntryViewModel(MainWindowViewModel mainWindow, ClassicServerStatusData server)
    {
        _mainWindow = mainWindow;
        _server = server;

        ConnectCommand = ReactiveCommand.Create(Connect);
    }

    private void Connect()
    {
        if (IsByondInstalled())
            Helpers.OpenUri(new Uri(Address));
        else
        {
            Log.Information("User attempted to connect to BYOND server but BYOND is not installed.");
            // Set the MainWindowViewModel's CustomInfo to show the BYOND not installed message
            // I didn't wanna make another dialog, reuse the generic thing :)
            _mainWindow.CustomInfo = new LauncherInfoManager.CustomInfo()
            {
                Message = LocalizationManager.Instance.GetString("tab-servers-byond-error-msg"),
                Description = LocalizationManager.Instance.GetString("tab-servers-byond-error-desc"),
                LinkText = LocalizationManager.Instance.GetString("tab-servers-byond-error-link-text"),
                Link = "https://www.byond.com/download/",
            };
        }
    }

    private bool IsByondInstalled()
    {
        #if WINDOWS
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Dantom\BYOND");
        return key != null;
        #elif LINUX
        // Ask xdg-mime if BYOND is registered
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "xdg-mime",
                Arguments = "query default x-scheme-handler/byond",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return !string.IsNullOrWhiteSpace(output);
        #elif MACOS
        return true; // No idea, they might have it, might not
        #endif
    }
}
