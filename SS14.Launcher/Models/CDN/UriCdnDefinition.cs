namespace SS14.Launcher.Models.CDN;

public record struct UriCdnDefinition(string Name)
{
    public static implicit operator UriCdnDefinition(string name) => new(name);
    public static implicit operator string(UriCdnDefinition definition) => definition.Name;

    public override string ToString()
    {
        return Name;
    }
}
