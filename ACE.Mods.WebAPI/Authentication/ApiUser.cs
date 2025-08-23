namespace ACE.Mods.WebAPI.Authentication;

public class ApiUser(ApiKey key) : IUser
{
    private ApiKey ApiKey { get; } = key;

    public string DisplayName => ApiKey.Name ?? "";

    public string Key => ApiKey.Key ?? "";

    public string[] Roles => ApiKey.Grants.Contains("ALL") ? AuthDB.AvailableGrants.ToArray() : ApiKey.Grants.ToArray();
}
