using System;
using System.Diagnostics;
using Microsoft.Win32;
using ReactiveUI;
using SS14.Launcher;
using SS14.Launcher.Localization;
using SS14.Launcher.Models.ServerStatus;
using SS14.Launcher.ViewModels;

namespace SS14.Launcher.ViewModels.MainWindowTabs;

public class ClassicServerEntryViewModel : ViewModelBase
{
    private readonly ClassicServerStatusData _server;

    public string Name => _server.Name;
    public string Address => _server.Address;
    public string PlayerCount => _server.PlayerCount.ToString();
    public string Status => _server.Status;

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> ConnectCommand { get; }

    public ClassicServerEntryViewModel(ClassicServerStatusData server)
    {
        _server = server;

        ConnectCommand = ReactiveCommand.Create(Connect);
    }

    private void Connect()
    {
        if (IsByondInstalled())
        {
            Helpers.OpenUri(new Uri(_server.Address));
        }
        else
        {
            // Prompt to download
            // We can use the native MessageBox helper from Helpers if available or just open the link.
            // Following the prompt instructions mostly literally: "prompted to install it first by going to this link"
            
            // On Windows we can use the MessageBox to be nicer. 
            // NOTE: Helper MessageBox returns int, 1 is usually OK.
            if (OperatingSystem.IsWindows())
            {
                var res = Helpers.MessageBoxHelper(
                    "BYOND not detected. You need the BYOND client to play Space Station 13. Go to download page?", 
                    "BYOND Missing", 
                    0x00000004 | 0x00000030); // MB_YESNO | MB_ICONWARNING
                
                if (res == 6) // IDYES
                {
                     Helpers.OpenUri(new Uri("https://www.byond.com/download/"));
                }
            }
            else
            {
                // Non-windows, just open the link? Or maybe they have it via Wine?
                // For now, let's open the link if we can't be sure, or maybe just try launching it?
                // The prompt was "Check if they have BYOND... If not... prompt".
                // Since I can't check on Linux easily, I'll assume they might not have it if I can't check.
                // But actually, opening the URI is the best 'try'.
                // Let's just try to open it on non-windows.
                 Helpers.OpenUri(new Uri(_server.Address));
            }
        }
    }

    private bool IsByondInstalled()
    {
        if (!OperatingSystem.IsWindows())
        {
            // On Linux/Mac, we can't easily check for BYOND (usually running under Wine).
            // We'll return true to let the OS/Wine try to handle the protocol.
            return true;
        }

        try
        {
            // Check for BYOND in Registry
            // HKCU\Software\Dantom\BYOND is the standard key.
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Dantom\BYOND");
            return key != null;
        }
        catch
        {
            return false;
        }
    }
}
