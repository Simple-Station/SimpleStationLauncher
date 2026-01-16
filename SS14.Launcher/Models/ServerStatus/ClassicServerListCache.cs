using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

    public async Task Refresh()
    {
        try
        {
            var response = await _http.GetStringAsync("http://www.byond.com/games/exadv1/spacestation13?format=text");
            // Log.Information("BYOND Response: {Response}", response);
            await File.WriteAllTextAsync("byond_dump.txt", response);
            var servers = ParseByondResponse(response);

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

    private List<ClassicServerStatusData> ParseByondResponse(string response)
    {
        var list = new List<ClassicServerStatusData>();
        using var reader = new StringReader(response);

        string? line;
        string? currentName = null;
        string? currentUrl = null;
        string? currentStatus = null;
        int currentPlayers = 0;

        // Simple state machine to parse the text format
        // The format uses 'world/ID' blocks for servers.

        bool inServerBlock = false;

        while ((line = reader.ReadLine()) != null)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            if (trimmed.StartsWith("world/"))
            {
                // If we were parsing a server, save it
                if (inServerBlock && currentUrl != null)
                {
                    // Name might be missing, try to extract from status or use URL
                    var name = currentName ?? ExtractNameFromStatus(currentStatus) ?? "Unknown Server";
                    var roundTime = ExtractRoundTimeFromStatus(currentStatus);
                    list.Add(new ClassicServerStatusData(name, currentUrl, currentPlayers, CleanStatus(currentStatus, name) ?? "", roundTime ?? "In-Lobby"));
                }
                
                // Reset for new server
                inServerBlock = true;
                currentName = null;
                currentUrl = null;
                currentStatus = null;
                currentPlayers = 0;
            }
            else if (inServerBlock)
            {
                if (trimmed.StartsWith("name ="))
                {
                    currentName = ParseStringValue(trimmed);
                }
                else if (trimmed.StartsWith("url ="))
                {
                    currentUrl = ParseStringValue(trimmed);
                }
                else if (trimmed.StartsWith("status ="))
                {
                    currentStatus = ParseStringValue(trimmed);
                }
                else if (trimmed.StartsWith("players = list("))
                {
                    // "players = list("Bob","Alice")"
                    // Just count the commas + 1, correcting for empty list "list()"
                    var content = trimmed.Substring("players = list(".Length);
                    if (content.EndsWith(")"))
                    {
                        content = content.Substring(0, content.Length - 1);
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            currentPlayers = 0;
                        }
                        else
                        {
                            // A simple Count(',') + 1 is risky if names contain commas, but usually they are quoted.
                            // However, parsing full CSV is safer but 'Splitting by ",' might be enough?
                            // Let's iterate and count quoted segments.
                            // Or simpler: Splitting by ',' is mostly fine for SS13 ckeys.
                            currentPlayers = content.Split(',').Length;
                        }
                    }
                }
                else if (trimmed.StartsWith("players ="))
                {
                    // Fallback for simple number if ever used
                   var parts = trimmed.Split('=');
                   if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out var p))
                   {
                       currentPlayers = p;
                   }
                }
            }
        }

        // Add the last one if exists
        if (inServerBlock && currentUrl != null)
        {
            var name = currentName ?? ExtractNameFromStatus(currentStatus) ?? "Unknown Server";
            var roundTime = ExtractRoundTimeFromStatus(currentStatus);
            list.Add(new ClassicServerStatusData(name, currentUrl, currentPlayers, CleanStatus(currentStatus, name) ?? "", roundTime ?? "In-Lobby"));
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

    private string ParseStringValue(string line)
    {
        // format: key = "value"
        var idx = line.IndexOf('"');
        if (idx == -1) return string.Empty;
        var lastIdx = line.LastIndexOf('"');
        if (lastIdx <= idx) return string.Empty;

        // Extract content inside quotes
        var inner = line.Substring(idx + 1, lastIdx - idx - 1);
        
        // Unescape BYOND/C string escapes
        // \" -> "
        // \n -> newline
        // \\ -> \
        // The most critical one is \n showing up as literal \n in UI.
        
        // Simple manual unescape for common sequences
        return inner.Replace("\\\"", "\"")
                    .Replace("\\n", "\n")
                    .Replace("\\\\", "\\")
                    .Replace("\\t", "\t");
    }
}
