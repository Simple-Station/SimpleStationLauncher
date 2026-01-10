using System;
using System.Collections.Generic;
using SS14.Launcher.Utility;
using TerraFX.Interop.Windows;

namespace SS14.Launcher;

public static class ConfigConstants
{
    public const string CurrentLauncherVersion = "3.4.2";
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

    public const string FallbackAuthServer = "Space-Wizards";
    public const string GuestAuthServer = "guest";
    public const string CustomAuthServer = "Custom";
    public static readonly AuthServer TemplateAuthServer = new(new("https://example.com/"), new("https://example.com/"));
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
            CustomAuthServer,
            new (new("https://example.com/"), new("https://example.com/"))
        },
    };

    public static readonly Uri[] DefaultHubUrls =
    [
        new("https://hub.simplestation.org/"),
        new("https://hub.singularity14.co.uk/"),
        new("https://cdn.spacestationmultiverse.com/hub/"),
        new("https://hub.spacestation14.com/"),
    ];

    public const string DiscordUrl = "https://discord.gg/49KeKwXc8g";
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
        {
            "Supermatter",
            new UrlFallbackSet([
                "https://cdn.simplestation.org/supermatter/manifest.json",
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


    public class AuthServer(
        Uri authUrl,
        Uri accountSite,
        bool? recommended = null,
        string authAuthPath = "api/auth/authenticate",
        string authRegPath = "api/auth/register",
        string authPwResetPath = "api/auth/resetPassword",
        string authResendPath = "api/auth/resendConfirmation",
        string authPingPath = "api/auth/ping",
        string authRefreshPath = "api/auth/refresh",
        string authLogoutPath = "api/auth/logout",
        string authAccountSitePath = "api/accountSite",
        string accountManPath = "Identity/Account/Manage",
        string accountRegPath = "Identity/Account/Register",
        string accountResendPath = "Identity/Account/ResendEmailConfirmation")
    {
        public bool? Recommended { get; } = recommended;

        public Uri AuthUrl { get; } = authUrl;
        public string AuthAuthPath { get; } = authAuthPath;
        public string AuthRegPath { get; } = authRegPath;
        public string AuthPwResetPath { get; } = authPwResetPath;
        public string AuthResendPath { get; } = authResendPath;
        public string AuthPingPath { get; } = authPingPath;
        public string AuthRefreshPath { get; } = authRefreshPath;
        public string AuthLogoutPath { get; } = authLogoutPath;
        public string AuthAccountSitePath { get; } = authAccountSitePath;
        public string AuthAuthUrl { get; } = $"{authUrl}{authAuthPath}";
        public string AuthRegUrl { get; } = $"{authUrl}{authRegPath}";
        public string AuthPwResetUrl { get; } = $"{authUrl}{authPwResetPath}";
        public string AuthResendUrl { get; } = $"{authUrl}{authResendPath}";
        public string AuthPingUrl { get; } = $"{authUrl}{authPingPath}";
        public string AuthRefreshUrl { get; } = $"{authUrl}{authRefreshPath}";
        public string AuthLogoutUrl { get; } = $"{authUrl}{authLogoutPath}";
        public string AuthAccountSiteUrl { get; } = $"{authUrl}{authAccountSitePath}";

        public Uri AccountSite { get; } = accountSite;
        public string AccountManPath { get; } = accountManPath;
        public string AccountRegPath { get; } = accountRegPath;
        public string AccountResendPath { get; } = accountResendPath;
        public string AccountManUrl { get; } = $"{accountSite}{accountManPath}";
        public string AccountRegUrl { get; } = $"{accountSite}{accountRegPath}";
        public string AccountResendUrl { get; } = $"{accountSite}{accountResendPath}";
    }
}
