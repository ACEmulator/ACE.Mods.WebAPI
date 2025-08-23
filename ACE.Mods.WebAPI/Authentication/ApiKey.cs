namespace ACE.Mods.WebAPI.Authentication;

public class ApiKey
{
    public string? Name { get; set; }
    public string? Key { get; set; }
    public HashSet<string> Grants { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);
}
