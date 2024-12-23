using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SS14.Launcher.Utility;

namespace SS14.Launcher.Models.EngineManager;

public sealed partial class EngineManagerDynamic
{
    // This part of the code is responsible for downloading and caching the Robust build manifest.

    private readonly SemaphoreSlim _manifestSemaphore = new(1);
    private readonly Stopwatch _manifestStopwatch = Stopwatch.StartNew();

    private readonly Dictionary<string, Dictionary<string, VersionInfo>?> _cachedEngineVersionInfo = new();
    private TimeSpan _robustCacheValidUntil;

    /// <summary>
    /// Look up information about an engine version.
    /// </summary>
    /// <param name="version">The version number to look up.</param>
    /// <param name="followRedirects">Follow redirections in version info.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>
    /// Information about the version, or null if it could not be found.
    /// The returned version may be different than what was requested if redirects were followed.
    /// </returns>
    private async ValueTask<FoundVersionInfo?> GetVersionInfo(
        string version,
        string engine,
        bool followRedirects = true,
        CancellationToken cancel = default)
    {
        await _manifestSemaphore.WaitAsync(cancel);
        try
        {
            return await GetVersionInfoCore(version, followRedirects, cancel, engine);
        }
        finally
        {
            _manifestSemaphore.Release();
        }
    }

    private async ValueTask<FoundVersionInfo?> GetVersionInfoCore(
        string version,
        bool followRedirects,
        CancellationToken cancel,
        string engine)
    {
        // First, check if we have a cached copy of the manifest.
        if (_cachedEngineVersionInfo.TryGetValue(engine, out var versionInfo)
            && versionInfo != null
            && _robustCacheValidUntil > _manifestStopwatch.Elapsed)
            return FindVersionInfoInCached(version, followRedirects, engine);

        // If we don't have a cached copy, or it's expired, we re-request the manifest.
        await UpdateBuildManifest(cancel, engine);
        return FindVersionInfoInCached(version, followRedirects, engine);
    }

    private async Task UpdateBuildManifest(CancellationToken cancel, string name)
    {
        // TODO: If-Modified-Since and If-None-Match request conditions.

        if (ConfigConstants.EngineBuildsUrl.TryGetValue(name, out var urlSet))
            foreach (var url in urlSet.Urls)
            {
                try
                {
                    _cachedEngineVersionInfo.Remove(name);
                    _cachedEngineVersionInfo.Add(name, await new UrlFallbackSet([url]).GetFromJsonAsync<Dictionary<string, VersionInfo>>(_http, cancel));
                    break;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to download manifest from {url}", url);
                }
            }

        _robustCacheValidUntil = _manifestStopwatch.Elapsed + ConfigConstants.RobustManifestCacheTime;
    }

    private FoundVersionInfo? FindVersionInfoInCached(string version, bool followRedirects, string name)
    {
        if (!_cachedEngineVersionInfo.TryGetValue(name, out var versionInfo))
            Debug.Assert(false);
        if (versionInfo == null || !versionInfo.TryGetValue(version, out var info))
            return null;

        if (followRedirects)
        {
            while (info.RedirectVersion != null)
            {
                if (!versionInfo.TryGetValue(info.RedirectVersion, out info))
                    return null;
            }
        }

        return new FoundVersionInfo(version, info);
    }

    private sealed record FoundVersionInfo(string Version, VersionInfo Info);

    private sealed record VersionInfo(
        bool Insecure,
        [property: JsonPropertyName("redirect")]
        string? RedirectVersion,
        Dictionary<string, BuildInfo> Platforms);

    private sealed class BuildInfo
    {
        [JsonInclude] [JsonPropertyName("url")]
        public string Url = default!;

        [JsonInclude] [JsonPropertyName("sha256")]
        public string Sha256 = default!;

        [JsonInclude] [JsonPropertyName("sig")]
        public string Signature = default!;
    }
}
