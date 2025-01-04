using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JetBrains.Annotations;
using Microsoft.Win32;
using Serilog;
using SS14.Launcher.Models.OverrideAssets;
using static System.Diagnostics.Process;
using static System.Environment.SpecialFolder;

namespace SS14.Launcher;

public class App : Application
{
    private static readonly Dictionary<string, AssetDef> AssetDefs = new()
    {
        ["WindowIcon"] = new AssetDef("icon.ico", AssetType.WindowIcon),
        ["LogoLong"] = new AssetDef("logo-long.png", AssetType.Bitmap),
    };

    private readonly OverrideAssetsManager _overrideAssets;

    private readonly Dictionary<string, object> _baseAssets = new();

    // XAML insists on a parameterless constructor existing, despite this never being used.
    [UsedImplicitly]
    public App()
    {
        throw new InvalidOperationException();
    }

    public App(OverrideAssetsManager overrideAssets)
    {
        _overrideAssets = overrideAssets;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        LoadBaseAssets();
        IconsLoader.Load(this);

        _overrideAssets.AssetsChanged += OnAssetsChanged;

        RegisterProtocol();
    }

    private void LoadBaseAssets()
    {
        foreach (var (name, (path, type)) in AssetDefs)
        {
            using var dataStream = AssetLoader.Open(new Uri($"avares://SS14.Launcher/Assets/{path}"));

            var asset = LoadAsset(type, dataStream);

            _baseAssets.Add(name, asset);
            Resources.Add(name, asset);
        }
    }

    private void OnAssetsChanged(OverrideAssetsChanged obj)
    {
        foreach (var (name, data) in obj.Files)
        {
            if (!AssetDefs.TryGetValue(name, out var def))
            {
                Log.Warning("Unable to find asset def for asset: '{AssetName}'", name);
                continue;
            }

            var ms = new MemoryStream(data, writable: false);
            var asset = LoadAsset(def.Type, ms);

            Resources[name] = asset;
        }

        // Clear assets not given to base data.
        foreach (var (name, asset) in _baseAssets)
        {
            if (!obj.Files.ContainsKey(name))
                Resources[name] = asset;
        }
    }

    private static object LoadAsset(AssetType type, Stream data)
    {
        return type switch
        {
            AssetType.Bitmap => new Bitmap(data),
            AssetType.WindowIcon => new WindowIcon(data),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void RegisterProtocol()
    {
        List<string> protocols = new() { "ss14", "ss14s" };

        #if WINDOWS
        Log.Information("Registering SS14 protocol handler for Windows");
        foreach (var protocol in protocols)
        {
            var key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{protocol}");
            key.SetValue(string.Empty, $"URL: {protocol}");
            key.SetValue("URL Protocol", string.Empty);

            key = key.CreateSubKey(@"shell\open\command");
            key.SetValue(string.Empty, $"\"{Environment.ProcessPath}\" \"%1\"");
            key.Close();
        }
        #else // Linux
        Log.Information("Registering SS14 protocol handler for Linux");
        foreach (var protocol in protocols)
        {
            try
            {
                // Put it in XDG_DATA_HOME/applications or ~/.local/share/applications
                var desktopFile = Path.Combine(
                    Environment.GetEnvironmentVariable("XDG_DATA_HOME")
                        ?? Path.Combine(Environment.GetFolderPath(UserProfile), ".local", "share"),
                    "applications", $"ss14-{protocol}.desktop");
                if (File.Exists(desktopFile))
                    #if DEBUG
                        File.WriteAllText(desktopFile, string.Empty);
                    #else
                        continue;
                    #endif

                using var writer = new StreamWriter(File.OpenWrite(desktopFile));
                writer.WriteLine("[Desktop Entry]");
                writer.WriteLine("Type=Application");
                writer.WriteLine($"Name=SS14 {protocol}");
                writer.WriteLine($"Exec=\"{Environment.ProcessPath}\" %u");
                writer.WriteLine("Terminal=false");
                writer.WriteLine($"MimeType=x-scheme-handler/{protocol};");
                writer.WriteLine("Categories=Network;");
                writer.Close();

                Log.Information($"Created SS14 protocol handler desktop file for Linux at {desktopFile}");
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to create SS14 protocol handler desktop file for Linux");
            }

            try
            {
                Start("xdg-mime", $"default ss14-{protocol}.desktop x-scheme-handler/{protocol}");
                Start("update-desktop-database", "~/.local/share/applications");

                Log.Information("Updated SS14 protocol handler registry for Linux");
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to update SS14 protocol handler registry for Linux");
            }
        }
        #endif
    }

    private sealed record AssetDef(string DefaultPath, AssetType Type);

    private enum AssetType
    {
        Bitmap,
        WindowIcon
    }
}
