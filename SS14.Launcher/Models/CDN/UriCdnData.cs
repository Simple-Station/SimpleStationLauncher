using System;

namespace SS14.Launcher.Models.CDN;

public readonly record struct UriCdnData(UriCdnDefinition Id, Uri Uri)
{
    public override string ToString()
    {
        return $"{Id.Name}={Uri.ToString()};";
    }
};
