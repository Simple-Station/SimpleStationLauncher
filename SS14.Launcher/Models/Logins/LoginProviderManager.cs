using System;
using Serilog;
using SS14.Launcher.Models.CDN;

namespace SS14.Launcher.Models.Logins;

public sealed class LoginProviderManager
{
    private readonly CdnManager _cdnManager;

    public LoginProviderManager(CdnManager cdnManager)
    {
        _cdnManager = cdnManager;
    }

    public ConfigConstants.AuthServer GetAuthServerById(string serverId, string? customAuthUrl = null, string? customAccountSite = null)
    {
        if (serverId != ConfigConstants.CustomAuthServer)
        {
            return _cdnManager.ResolveDefinition(ConfigConstants.AuthUrls[serverId]);
        }

        if (customAuthUrl == null)
            throw new ArgumentException("Custom server selected but no custom URLs provided.");

        customAccountSite ??= TryGetAccountUrl(serverId, customAuthUrl);
        if (customAccountSite == null)
            throw new ArgumentException("Failed to get account URL for custom server.");

        return new ConfigConstants.AuthServer(new(customAuthUrl), new(customAccountSite));
    }

    public string? TryGetAccountUrl(string serverId, string? customAuthUrl = null)
    {
        if (serverId != ConfigConstants.CustomAuthServer)
            return GetAuthServerById(serverId).AccountSite.ToString();

        if (customAuthUrl == null)
            throw new ArgumentException("Custom server selected but no custom URLs provided.");

        // Make an http request to the custom URL to get the account URL
        var http = HappyEyeballsHttp.CreateHttpClient();
        var response = http.GetAsync(new Uri(customAuthUrl) + ConfigConstants.TemplateAuthServer.AuthAccountSitePath).Result;
        http.Dispose();
        if (!response.IsSuccessStatusCode)
        {
            Log.Error("Failed to get account URL from custom auth server with status {status}", response.StatusCode);
            return null;
        }

        return response.Content.AsJson<AccountSiteResponse>().Result.WebBaseUrl;
    }

    private sealed record AccountSiteResponse(string WebBaseUrl);
}
