using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Serilog;
using SS14.Launcher.Controls.CDN;
using SS14.Launcher.Models.Data;

namespace SS14.Launcher.Models.CDN;

public class CdnManager
{
    private readonly List<UriCdnData> _cdnList;
    private readonly Dictionary<UriCdnDefinition, UriCdnData> _cdnMap = [];

    private static readonly PingCache Cache = new();

    private static readonly string DataPath = Path.Combine(LauncherPaths.DirUserData, "mirrors.txt");

    public CdnManager()
    {
        using var fileStream = File.Open(DataPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
        using var streamReader = new StreamReader(fileStream);

        _cdnList = CdnDataListSerializer.DeserializeCdnList(streamReader.ReadToEnd()).ToList();

        if (_cdnList.Count == 0)
        {
            _cdnList = CdnHelper.DefaultCdnList;
            Dirty();
        }
    }

    public IEnumerable<Uri> ResolveDefinition(IEnumerable<UriCdnDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            yield return ResolveDefinition(definition);
        }
    }

    public Uri ResolveDefinition(UriCdnDefinition definition)
    {
        if (_cdnMap.TryGetValue(definition, out var cdnData)) return cdnData.Uri;

        if (Uri.TryCreate(definition.Name, UriKind.Absolute, out var uri))
        {
            Log.Warning("Trying to resolve url, not definition: {definition}", definition);
            return uri;
        }

        throw new KeyNotFoundException($"Cdn definition {definition} not found");
    }

    private CdnPingWindow _pingWindow = default!;

    public void ShowPingWindow()
    {
        _pingWindow = new CdnPingWindow();
        _pingWindow.Show();
    }

    public async Task SortFastestAndMap()
    {
        Log.Information("Resolving all CDN data with length: {0}", _cdnList.Count);

        _cdnMap.Clear();

        var compoundMap = new Dictionary<UriCdnDefinition, List<CdnDataCompound>>();

        foreach (var cdnData in _cdnList)
        {
            var dataCompound = new CdnDataCompound(cdnData);

            if (!compoundMap.TryGetValue(cdnData.Id, out var compoundDataList))
            {
                compoundDataList = new List<CdnDataCompound>();
                compoundMap.Add(cdnData.Id, compoundDataList);
            }

            compoundDataList.Add(dataCompound);
        }

        foreach (var (definition, list) in compoundMap.ToDictionary())
        {
            switch (list.Count)
            {
                case 0:
                    compoundMap.Remove(definition);
                    continue;
                case 1:
                    _cdnMap[definition] = list[0].CdnData;
                    Log.Information($"Skip {definition} because is have only one cdn data: {list[0].CdnData}");
                    compoundMap.Remove(definition);
                    continue;
            }

            Dispatcher.UIThread.Post(() =>
            {
                foreach (var compound in list)
                {
                    _pingWindow.ResolveItem(compound);
                }
            });
        }

        var compoundList = compoundMap.SelectMany(a => a.Value).ToList();
        Log.Information("Resolving fastest CDN. Count: {0}", compoundList.Count());

        var compoundTask = compoundList.Select(Ping);

        await Task.WhenAll(compoundTask);

        foreach (var (key, value) in compoundMap)
        {
            value.Sort((a,b) => a.CompareTo(b));
            var fastest = value.First();

            Log.Information("Resolved CDN data " + fastest.CdnData);
            _cdnMap[key] = fastest.CdnData;
        }

        Log.Information("Resolved all CDN data");
    }

    private async Task Ping(CdnDataCompound compound)
    {
        compound.Ping = await Cache.GetPingAsync(compound.CdnData.Uri.Host);
        _pingWindow.ResolveItem(compound);
    }

    private void Dirty()
    {
        File.WriteAllText(DataPath, CdnDataListSerializer.SerializeCdnList(_cdnList));
    }
}

public sealed class CdnDataCompound: IComparable<CdnDataCompound>
{

    public UriCdnData CdnData { get; set; }
    public CdnPingResponse Ping { get; set; } = new CdnPingResponse(null, "Resolving");

    public CdnDataCompound(UriCdnData cdnData)
    {
        CdnData = cdnData;
    }

    public int CompareTo(CdnDataCompound? other)
    {
        if (ReferenceEquals(other, null))
            return -1;

        if (Ping.TimeoutMs.HasValue && other.Ping.TimeoutMs.HasValue)
        {
            int cmp = Ping.TimeoutMs.Value.CompareTo(other.Ping.TimeoutMs.Value);
            if (cmp != 0)
                return cmp;
        }
        else if (Ping.TimeoutMs.HasValue)
        {
            return -1;
        }
        else if (other.Ping.TimeoutMs.HasValue)
        {
            return 1;
        }

        return string.Compare(
            CdnData.Uri.ToString(),
            other.CdnData.Uri.ToString(),
            StringComparison.OrdinalIgnoreCase);
    }
}

public class PingCache
{
    private readonly ConcurrentDictionary<string, CdnPingResponse> _cache = new();
    private readonly ConcurrentDictionary<string, Task<CdnPingResponse>> _resolvingCache = new();

    public Task<CdnPingResponse> GetPingAsync(string host)
    {
        if (_resolvingCache.TryGetValue(host, out var cacheTask))
        {
            return cacheTask;
        }

        if (_cache.TryGetValue(host, out var entry))
        {
            return Task.FromResult(entry);
        }

        var task = InternalGetPingAsync(host);

        _resolvingCache.TryAdd(host, task);

        return task;
    }

    private async Task<CdnPingResponse> InternalGetPingAsync(string host)
    {
        using var ping = new Ping();
        try
        {
            var reply = await ping.SendPingAsync(host, TimeSpan.FromMilliseconds(700));

            var resp = reply.Status == IPStatus.Success
                ? new CdnPingResponse((int)reply.RoundtripTime, "Successful")
                : new CdnPingResponse(null, reply.Status.ToString(), true);

            _cache[host] = resp;
            return resp;
        }
        catch (Exception ex)
        {
            var resp = new CdnPingResponse(null, ex.Message, true);
            _cache[host] = resp;
            return resp;
        }
        finally
        {
            _resolvingCache.TryRemove(host, out _);
        }
    }
}

public record struct CdnPingResponse(int? TimeoutMs, string Reason = "", bool Error = false);

