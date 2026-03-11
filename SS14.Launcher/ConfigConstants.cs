using System;
using System.Collections.Generic;
using SS14.Launcher.Models.CDN;
using SS14.Launcher.Utility;

namespace SS14.Launcher;

public static class ConfigConstants
{
    public const string CurrentLauncherVersion = "3.6.1";
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

    public static readonly UriCdnDefinition FallbackAuthServer = "Space-Wizards";
    public static readonly UriCdnDefinition GuestAuthServer = "guest";
    public static readonly UriCdnDefinition CustomAuthServer = "Custom";
    public static readonly AuthServer TemplateAuthServer = new(new("https://example.com/"), new("https://example.com/"));

    public static readonly Dictionary<string, AuthServerDefinition> AuthUrls = new()
    {
        {
            "SimpleStation",
            new("SimpleStationAuth", "SimpleStationAccount", true)
        },
        {
            FallbackAuthServer,
            new("FallbackAuthServerAuth", "FallbackAuthServerAccount", false)
        },
        {
            CustomAuthServer,
            new ("CustomAuthServerAuth", "CustomAuthServerAccount")
        },
    };

    public static readonly UriCdnDefinition[] DefaultHubUrls =
    [
        "SimpleStationHub",
        "SingularityHub",
        "MultiverseHub",
        "SpaceStationHub",
    ];

    public const string DiscordUrl = "https://discord.gg/49KeKwXc8g";
    public const string WebsiteUrl = "https://simplestation.org";
    public const string DownloadUrl = "https://github.com/Simple-Station/SimpleStationLauncher/releases/";
    public const string NewsFeedUrl = "https://spacestation14.com/post/index.xml"; //TODO
    public const string TranslateUrl = "https://docs.spacestation14.com/en/general-development/contributing-translations.html"; //TODO

    public static readonly Dictionary<string, UriCdnDefinition> EngineBuildsUrl = new()
    {
        {
            "Robust", "RobustEngine"
        },
        {
            "Multiverse", "MultiverseEngine"
        },
        {
            "Supermatter", "SupermatterEngine"
        },
    };

    public static readonly Dictionary<string, UriCdnDefinition> EngineModulesUrl = new()
    {
        {
            "Robust", "RobustModules"
        },
        {
            "Multiverse", "MultiverseModules"
        },
    };

    // How long to keep cached copies of Robust manifests.
    // TODO: Take this from Cache-Control header responses instead.
    public static readonly TimeSpan RobustManifestCacheTime = TimeSpan.FromMinutes(15);

    public static readonly UriCdnDefinition UrlLauncherInfo = "LauncherInfo";
    public static readonly UriCdnDefinition UrlAssetsBase = "LauncherAssetsBase";

    public const string FallbackUsername = "JoeGenero";

    public record struct AuthServerDefinition(
        UriCdnDefinition AuthUrl,
        UriCdnDefinition AccountSite,
        bool? Recommended = null);

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
