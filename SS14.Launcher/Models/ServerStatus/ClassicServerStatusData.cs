using System.Collections.Generic;

namespace SS14.Launcher.Models.ServerStatus;

public class ClassicServerStatusData
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int PlayerCount { get; set; }
    public string Status { get; set; } = string.Empty;

    public ClassicServerStatusData(string name, string address, int playerCount, string status)
    {
        Name = name;
        Address = address;
        PlayerCount = playerCount;
        Status = status;
    }
}
