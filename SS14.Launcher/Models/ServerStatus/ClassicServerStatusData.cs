namespace SS14.Launcher.Models.ServerStatus;

public class ClassicServerStatusData(string name, string address, int playerCount, string status, string roundTime)
{
    public string Name { get; } = name;
    public string Address { get; } = address;
    public int PlayerCount { get; } = playerCount;
    public string Status { get; } = status;
    public string RoundTime { get; } = roundTime;
}
