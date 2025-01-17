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

    // Yes, this is stupidly large, just collapse it if you don't want to see it
    public class AuthServer(
        Uri authUrl,
        Uri accountSite,
        bool? recommended = null,
        string authAuthUrl = "api/auth/authenticate",
        string authRegUrl = "api/auth/register",
        string authPwResetUrl = "api/auth/resetPassword",
        string authResendUrl = "api/auth/resendConfirmation",
        string authPingUrl = "api/auth/ping",
        string authRefreshUrl = "api/auth/refresh",
        string authLogoutUrl = "api/auth/logout",
        string accountManUrl = "Manage",
        string accountRegUrl = "Register",
        string accountResendUrl = "ResendEmailConfirmation")
    {
        public bool? Recommended { get; } = recommended;

        public Uri AuthUrl { get; } = authUrl;
        public string AuthAuthUrl { get; } = authAuthUrl;
        public string AuthRegUrl { get; } = authRegUrl;
        public string AuthPwResetUrl { get; } = authPwResetUrl;
        public string AuthResendUrl { get; } = authResendUrl;
        public string AuthPingUrl { get; } = authPingUrl;
        public string AuthRefreshUrl { get; } = authRefreshUrl;
        public string AuthLogoutUrl { get; } = authLogoutUrl;
        public string AuthAuthFullUrl { get; } = $"{authUrl}{authAuthUrl}";
        public string AuthRegFullUrl { get; } = $"{authUrl}{authRegUrl}";
        public string AuthPwResetFullUrl { get; } = $"{authUrl}{authPwResetUrl}";
        public string AuthResendFullUrl { get; } = $"{authUrl}{authResendUrl}";
        public string AuthPingFullUrl { get; } = $"{authUrl}{authPingUrl}";
        public string AuthRefreshFullUrl { get; } = $"{authUrl}{authRefreshUrl}";
        public string AuthLogoutFullUrl { get; } = $"{authUrl}{authLogoutUrl}";

        public Uri AccountSite { get; } = accountSite;
        public string AccountManUrl { get; } = accountManUrl;
        public string AccountRegUrl { get; } = accountRegUrl;
        public string AccountResendUrl { get; } = accountResendUrl;
        public string AccountManFullUrl { get; } = $"{accountSite}{accountManUrl}";
        public string AccountRegFullUrl { get; } = $"{accountSite}{accountRegUrl}";
        public string AccountResendFullUrl { get; } = $"{accountSite}{accountResendUrl}";
    }
    public const string FallbackAuthServer = "Space-Wizards";
    public static readonly Dictionary<string, AuthServer> AuthUrls = new()
    {
        {
            "SimpleStation",
            new(new("https://auth.simplestation.org/"), new("https://account.simplestation.org/"), true)
        },
        {
            FallbackAuthServer,
            new(new("https://auth.spacestation14.com/"), new("https://account.spacestation14.com/"), false)
        },
        {
            "Custom",
            new (new("https://example.com/"), new("https://example.com/"))
        },
    };

    public static readonly Uri[] DefaultHubUrls =
    [
        new("https://web.networkgamez.com/"),
        new("https://hub.singularity14.co.uk/"),
        new("https://cdn.spacestationmultiverse.com/hub/"),
        new("https://hub.spacestation14.com/"),
    ];

    public const string DiscordUrl = "https://discord.gg/49KeKwXc8g/";
    public const string WebsiteUrl = "https://simplestation.org";
    public const string DownloadUrl = "https://github.com/Simple-Station/SimpleStationLauncher/releases/";
    public const string NewsFeedUrl = "https://spacestation14.com/post/index.xml"; //TODO
    public const string TranslateUrl = "https://docs.spacestation14.com/en/general-development/contributing-translations.html"; //TODO

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
}
