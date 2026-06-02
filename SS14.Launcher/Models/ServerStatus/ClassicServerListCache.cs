using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Serilog;
using Splat;
using SS14.Launcher.Utility;

namespace SS14.Launcher.Models.ServerStatus;

public sealed class ClassicServerListCache
{
    private readonly HttpClient _http;
    private readonly ObservableCollection<ClassicServerStatusData> _allServers = new();

    public ReadOnlyObservableCollection<ClassicServerStatusData> AllServers { get; }

    public ClassicServerListCache()
    {
        _http = Locator.Current.GetRequiredService<HttpClient>();
        AllServers = new ReadOnlyObservableCollection<ClassicServerStatusData>(_allServers);
    }

    public ClassicServerStatusData GetStatusFor(string address)
    {
        foreach (var server in _allServers)
            if (server.Address == address)
                return server;

        return new ClassicServerStatusData("Unknown Server", address, 0, "Fetching...", "");
    }

    public async Task Refresh()
    {
        try
        {
            var hubResponse = await _http.GetFromJsonAsync<GoonhubHubResponse>("https://node.goonhub.com/hub");
            if (hubResponse == null) return;

            var servers = ParseGoonhubResponse(hubResponse);

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                _allServers.Clear();
                foreach (var server in servers)
                {
                    _allServers.Add(server);
                }
            });
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to fetch Classic SS13 server list.");
        }
    }

    private List<ClassicServerStatusData> ParseGoonhubResponse(GoonhubHubResponse hubResponse)
    {
        var list = new List<ClassicServerStatusData>();
        foreach (var server in hubResponse.Response)
        {
            var name = ExtractNameFromStatus(server.Status) ?? "Unknown Server";
            var roundTime = ExtractRoundTimeFromStatus(server.Status) ?? "In-Lobby";
            var address = $"byond://BYOND.world.{Uri.EscapeDataString(server.UrlId)}";
            
            list.Add(new ClassicServerStatusData(name, address, server.Players, CleanStatus(server.Status, name) ?? "", roundTime));
        }
        return list;
    }

    private string? ExtractRoundTimeFromStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return null;

        // Try to match "Round time: <b>00:07</b>" or similar
        var match = System.Text.RegularExpressions.Regex.Match(status, @"Round\s+time:\s+(?:<b>)?(\d{1,2}:\d{2})(?:</b>)?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        return null;
    }

    private string? ExtractNameFromStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return null;
        // Usually starts with <b>Name</b>
        var match = System.Text.RegularExpressions.Regex.Match(status, @"<b>(.*?)</b>");
        if (match.Success)
        {
            var raw = match.Groups[1].Value;
             // Remove nested tags if any
             var clean = System.Text.RegularExpressions.Regex.Replace(raw, "<.*?>", String.Empty);
             return System.Net.WebUtility.HtmlDecode(clean);
        }
        return null;
    }

    private string? CleanStatus(string? status, string? nameToRemove)
    {
        if (string.IsNullOrEmpty(status)) return null;

        var s = status.Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n");
        // Remove tags
        s = System.Text.RegularExpressions.Regex.Replace(s, "<.*?>", String.Empty);

        // Decode HTML
        s = System.Net.WebUtility.HtmlDecode(s);

        if (nameToRemove != null && s.StartsWith(nameToRemove))
        {
            s = s.Substring(nameToRemove.Length);
        }

        // Clean artifacts
        char[] trims = { ' ', '\t', '\n', '\r', ']', ')', '-', '—', ':' };
        s = s.TrimStart(trims).Trim();

        // Reduce multiple newlines
        s = System.Text.RegularExpressions.Regex.Replace(s, @"\n\s+", "\n");
        s = System.Text.RegularExpressions.Regex.Replace(s, @"\n{3,}", "\n\n");

        return s;
    }

    private sealed class GoonhubHubResponse
    {
        [JsonPropertyName("response")] public List<GoonhubServerEntry> Response { get; set; } = new();
    }

    private sealed class GoonhubServerEntry
    {
        [JsonPropertyName("urlId")] public string UrlId { get; set; } = "";
        [JsonPropertyName("players")] public int Players { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; } = "";
    }
}
