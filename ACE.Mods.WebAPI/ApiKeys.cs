namespace ACE.Mods.WebAPI;

public class APIKeys
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        WriteIndented = true,
        //PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //PropertyNameCaseInsensitive = true

        //WriteIndented = true,
        //AllowTrailingCommas = true,
        //Converters = { new JsonStringEnumConverter() },
        //Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //IncludeFields = true,
        //UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private static readonly string fileName = "APIKeys.json";
    private static readonly string filePath = Mod.Instance.ModPath + "/" + fileName;

    //public static List<APIKey> Keys { get; set; } = new List<APIKey>();

    public static Dictionary<string, APIKey> Keys { get; set; } = new();

    public static void Load()
    {
        if (File.Exists(filePath))
        {
            // Try loading the keys from file
            using (FileStream fs = File.OpenRead(filePath))
            {
                Keys = JsonSerializer.Deserialize<List<APIKey>>(fs, jsonSerializerOptions)?.ToDictionary(k => k.Key) ?? new();                
            }
            foreach (var key in Keys.Keys)
            {
                Keys[key].Grants = new HashSet<string>(Keys[key].Grants, StringComparer.InvariantCultureIgnoreCase);
            }
        }
    }

    public static void Save()
    {
        using (FileStream fs = File.Create(filePath))
        {
            JsonSerializer.Serialize((Stream)fs, jsonSerializerOptions);
        }
    }

    public static bool Add(string name, string[] grants, out APIKey key, string apiKey = "")
    {
        key = new APIKey();

        if (HasKeyName(name))
            return false;

        return true;
    }

    public static bool HasKey(string apiKey) => Keys.ContainsKey(apiKey);

    public static bool HasKeyName(string name) => Keys.Values.Any(k => k.Name == name);

    public static APIKey? GetKeyByName(string name) => Keys.Values.FirstOrDefault(k => k.Name == name);

    public static HashSet<string> AvailableGrants { get; set; } = new() { "ALL" };

    public static void AddRolesToAvailableGrants<T>()
    {
        foreach (var method in typeof(T).GetMethods())
        {
            object[] methodAttributes = Attribute.GetCustomAttributes(method, typeof(RequireRoleAttribute));

            if (methodAttributes.Length > 0)
            {
                foreach (object attr in methodAttributes)
                {
                    //Console.WriteLine($"    Attribute: {attr.GetType().Name}");
                    if (attr is RequireRoleAttribute myCustomAttr)
                    {
                        //Console.WriteLine($"      Custom Name: {myCustomAttr.Roles}");
                        foreach (var role in myCustomAttr.Roles)
                        {
                            //Console.WriteLine($"      Custom role: {role}");
                            AvailableGrants.Add(role);
                        }
                    }
                }
            }
        }
    }

    public static string GenerateSecureApiKey(int byteLength = 32)
    {
        // Generate cryptographically secure random bytes
        byte[] bytes = RandomNumberGenerator.GetBytes(byteLength);

        // Convert the bytes to a URL-safe Base64 string
        // Replace '+' with '-' and '/' with '_' to ensure URL safety
        string base64String = Convert.ToBase64String(bytes)
                               .Replace('+', '-')
                               .Replace('/', '_');

        // Remove any padding characters ('=')
        return base64String.TrimEnd('=');
    }
}

public class APIKey
{
    public string? Name { get; set; }
    public string? Key { get; set; }
    public HashSet<string> Grants { get; set; }
}
