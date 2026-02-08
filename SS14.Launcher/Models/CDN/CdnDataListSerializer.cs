using System;
using System.Collections.Generic;
using System.Text;

namespace SS14.Launcher.Models.CDN;

public static class CdnDataListSerializer
{
    public static IEnumerable<UriCdnData> DeserializeCdnList(string raw)
    {
        foreach (var splited in raw.Split(';'))
        {
            var nameValue = splited.Split('=');
            if(nameValue.Length != 2) continue;
            yield return new UriCdnData(nameValue[0], new Uri(nameValue[1]));
        }
    }

    public static string SerializeCdnList(IEnumerable<UriCdnData> cdnList)
    {
        var str = "";
        foreach (var data in cdnList)
        {
            str += data.ToString();
        }
        return str;
    }
}
