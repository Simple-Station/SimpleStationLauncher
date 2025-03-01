using System;
using System.Collections.Generic;
using SS14.Launcher.Utility;

namespace SS14.Launcher;

public static class ConfigConstants
{
    public const string CurrentLauncherVersion = "1.3.1";
    #if RELEASE
    public const bool DoVersionCheck = true;
    #else
    public const bool DoVersionCheck = false;
    #endif

    // Refresh login tokens if they're within <this much> of expiry.
    public static readonly TimeSpan TokenRefreshThreshold = TimeSpan.FromDays(15);

    // If the user leaves the launcher running for absolute ages, this is how often we'll update his login tokens.
    public static readonly TimeSpan TokenRefreshInterval = TimeSpan.FromDays(7);

    // The amount of time before a server is considered timed out for status checks.
    public static readonly TimeSpan ServerStatusTimeout = TimeSpan.FromSeconds(5);

    // Check the command queue this often.
    public static readonly TimeSpan CommandQueueCheckInterval = TimeSpan.FromSeconds(1);

    public const string LauncherCommandsNamedPipeName = "SS14.Launcher.CommandPipe";
    // Amount of time to wait before the launcher decides to ignore named pipes entirely to keep the rest of the launcher functional.
    public const int LauncherCommandsNamedPipeTimeout = 150;
    // Amount of time to wait to let a redialling client properly die
    public const int LauncherCommandsRedialWaitTimeout = 1000;

    public static readonly string AuthUrl = "https://auth.spacestation14.com/";
    public static readonly Uri[] DefaultHubUrls =
    {
        new("https://cdn.spacestationmultiverse.com/hub/"),
        new("https://web.networkgamez.com/"),
        new("https://hub.singularity14.co.uk/"),
        new("https://hub.spacestation14.com/"),
    };
    public const string DiscordUrl = "https://discord.gg/Z5PsYHY6fM";
    public const string AccountBaseUrl = "https://account.spacestation14.com/Identity/Account/";
    public const string AccountManagementUrl = $"{AccountBaseUrl}Manage";
    public const string AccountRegisterUrl = $"{AccountBaseUrl}Register";
    public const string AccountResendConfirmationUrl = $"{AccountBaseUrl}ResendEmailConfirmation";
    public const string WebsiteUrl = "https://simplestation.org";
    public const string DownloadUrl = "https://github.com/Simple-Station/SimpleStationLauncher/releases/";
    public const string NewsFeedUrl = "https://spacestation14.com/post/index.xml";
    public const string TranslateUrl = "https://docs.spacestation14.com/en/general-development/contributing-translations.html";

    public static readonly Dictionary<string, UrlFallbackSet> EngineBuildsUrl = new()
    {
        {
            "Robust",
            new UrlFallbackSet([
                "https://robust-builds.cdn.spacestation14.com/manifest.json",
                "https://robust-builds.fallback.cdn.spacestation14.com/manifest.json",
            ])
        },
        {
            "Multiverse",
            new UrlFallbackSet([
                "https://cdn.spacestationmultiverse.com/ssmv-engine-manifest",
            ])
        },
    };

    public static readonly Dictionary<string, UrlFallbackSet> EngineModulesUrl = new()
    {
        {
            "Robust",
            new UrlFallbackSet([
                "https://robust-builds.cdn.spacestation14.com/modules.json",
                "https://robust-builds.fallback.cdn.spacestation14.com/modules.json",
            ])
        },
        {
            "Multiverse",
            new UrlFallbackSet([
                // Same as Robust for now
                "https://robust-builds.cdn.spacestation14.com/modules.json",
                "https://robust-builds.fallback.cdn.spacestation14.com/modules.json",
            ])
        },
    };

    private static readonly UrlFallbackSet LauncherDataBaseUrl = new([
        "http://assets.simplestation.org/launcher/",
    ]);

    // How long to keep cached copies of Robust manifests.
    // TODO: Take this from Cache-Control header responses instead.
    public static readonly TimeSpan RobustManifestCacheTime = TimeSpan.FromMinutes(15);

    public static readonly UrlFallbackSet UrlLauncherInfo = LauncherDataBaseUrl + "info.json";
    public static readonly UrlFallbackSet UrlAssetsBase = LauncherDataBaseUrl + "assets/";

    public const string FallbackUsername = "JoeGenero";

    static ConfigConstants()
    {
        var envVarAuthUrl = Environment.GetEnvironmentVariable("SS14_LAUNCHER_OVERRIDE_AUTH");
        if (!string.IsNullOrEmpty(envVarAuthUrl))
            AuthUrl = envVarAuthUrl;
    }
}
