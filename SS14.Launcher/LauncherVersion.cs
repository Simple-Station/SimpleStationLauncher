using System;

namespace SS14.Launcher;

public static class LauncherVersion
{
    public const string Name = "SimpleStationLauncher";
    public static Version? Version => typeof(LauncherVersion).Assembly.GetName().Version;
}
