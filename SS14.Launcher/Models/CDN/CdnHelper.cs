using System;
using System.Collections.Generic;
using SS14.Launcher.Utility;

namespace SS14.Launcher.Models.CDN;

public static class CdnHelper
{
    public static readonly List<UriCdnData> DefaultCdnList = new()
    {
        //AUTH
        new UriCdnData("SimpleStationAuth", new Uri("https://auth.simplestation.org/")),
        new UriCdnData("SimpleStationAccount", new Uri("https://account.simplestation.org/")),

        new UriCdnData("FallbackAuthServerAuth", new Uri("https://auth.spacestation14.com/")),
        new UriCdnData("FallbackAuthServerAccount", new Uri("https://account.spacestation14.com/")),

        new UriCdnData("CustomAuthServerAuth", new Uri("https://auth.example.com/")),
        new UriCdnData("CustomAuthServerAccount", new Uri("https://account.example.com/")),

        //HUB
        new UriCdnData("SimpleStationHub",new Uri("https://hub.simplestation.org/")),
        new UriCdnData("SingularityHub",new Uri("https://hub.singularity14.co.uk/")),
        new UriCdnData("MultiverseHub",new Uri("https://cdn.spacestationmultiverse.com/hub/")),
        new UriCdnData("SpaceStationHub",new Uri("https://hub.spacestation14.com/")),

        //ENGINE
        new UriCdnData("RobustEngine", new Uri("https://robust-builds.cdn.spacestation14.com/manifest.json")),
        new UriCdnData("RobustEngine", new Uri("https://robust-builds.fallback.cdn.spacestation14.com/manifest.json")),

        new UriCdnData("MultiverseEngine", new Uri("https://cdn.spacestationmultiverse.com/ssmv-engine-manifest")),
        new UriCdnData("SupermatterEngine", new Uri("https://cdn.simplestation.org/supermatter/manifest.json")),

        //MODULES
        new UriCdnData("RobustModules", new Uri("https://robust-builds.cdn.spacestation14.com/modules.json")),
        new UriCdnData("RobustModules", new Uri("https://robust-builds.fallback.cdn.spacestation14.com/modules.json")),
        // Same as Robust for now
        new UriCdnData("MultiverseModules", new Uri("https://robust-builds.cdn.spacestation14.com/modules.json")),
        new UriCdnData("MultiverseModules", new Uri("https://robust-builds.fallback.cdn.spacestation14.com/modules.json")),

        //Launcher assets
        new UriCdnData("LauncherInfo", new Uri("http://assets.simplestation.org/launcher/info.json")),
        new UriCdnData("LauncherAssetsBase", new Uri("http://assets.simplestation.org/launcher/assets/")),
    };

    public static ConfigConstants.AuthServer ResolveDefinition(this CdnManager cdnManager, ConfigConstants.AuthServerDefinition definition)
    {
        return new ConfigConstants.AuthServer(
            cdnManager.ResolveDefinition(definition.AuthUrl),
            cdnManager.ResolveDefinition(definition.AccountSite),
            definition.Recommended
        );
    }

    public static UrlFallbackSet ResolveUrlSet(this CdnManager cdnManager, UriCdnDefinition definition)
    {
        return new UrlFallbackSet([cdnManager.ResolveDefinition(definition).AbsoluteUri]);
    }

    public static IEnumerable<Uri> ResolveDefinition(this CdnManager cdnManager, IEnumerable<UriCdnDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            yield return cdnManager.ResolveDefinition(definition);
        }
    }
}
