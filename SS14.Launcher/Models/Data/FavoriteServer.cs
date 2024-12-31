using System.Text.Json.Serialization;
using ReactiveUI;

namespace SS14.Launcher.Models.Data;

public sealed class FavoriteServer : ReactiveObject
{
    private string? _name;
    private int _position;

    // For serialization.
    public FavoriteServer()
    {
        Address = default!;
    }

    public FavoriteServer(string? name, string address)
    {
        Name = name;
        Address = address;
    }

    public FavoriteServer(string? name, string address, int position)
    {
        Name = name;
        Address = address;
        Position = position;
    }

    [JsonPropertyName("name")]
    public string? Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    [JsonPropertyName("address")]
    public string Address { get; private set; } // Need private set for JSON.NET to work.

    /// <summary>
    /// Used to infer an exact ordering for servers in a simple, compatible manner.
    /// Defaults to 0, this is fine.
    /// This isn't saved in JSON because the launcher apparently doesn't use JSON for these anymore.
    /// </summary>
    public int Position
    {
        get => _position;
        set => this.RaiseAndSetIfChanged(ref _position, value);
    }
}
