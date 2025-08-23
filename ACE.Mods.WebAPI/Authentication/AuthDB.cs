namespace ACE.Mods.WebAPI.Authentication
{
    public class AuthDB
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        private static readonly string fileName = "APIKeys.json";
        private static readonly string filePath = Mod.Instance.ModPath + "/" + fileName;

        public static Dictionary<string, ApiKey> Keys { get; set; } = new();

        public static void Load()
        {
            if (File.Exists(filePath))
            {
                // Try loading the keys from file
                using (FileStream fs = File.OpenRead(filePath))
                {
                    Keys = JsonSerializer.Deserialize<List<ApiKey>>(fs, jsonSerializerOptions)?.ToDictionary(k => k.Key ?? "") ?? new();
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
                JsonSerializer.Serialize((Stream)fs, Keys.Values, jsonSerializerOptions);
            }
        }

        public static bool Add(string name, string[] grants, out ApiKey key, string apiKey = "")
        {
            key = new ApiKey();

            if (HasKeyName(name))
                return false;

            key.Name = name;

            if (apiKey == "")
                key.Key = GenerateSecureApiKey();
            else
                key.Key = apiKey;

            if (Keys.ContainsKey(key.Key))
                return false;

            foreach (var grant in grants)
                key.Grants.Add(grant);

            Keys.Add(key.Key, key);

            Save();

            return true;
        }

        public static bool Remove(string name)
        {
            var apiKey = GetKeyByName(name);

            if (apiKey == null || apiKey.Key == null)
                return false;

            Keys.Remove(apiKey.Key);

            Save();

            return true;
        }

        public static bool Modify(ApiKey key)
        {
            if (key?.Name == null || key?.Key == null)
                return false;

            var apiKey = Keys[key.Key] = key;

            if (apiKey == null || apiKey.Key == null)
                return false;

            Save();

            return true;
        }

        public static bool HasKey(string apiKey) => Keys.ContainsKey(apiKey);

        public static bool HasKeyName(string name) => Keys.Values.Any(k => k.Name == name);

        public static ApiKey? GetKeyByName(string name) => Keys.Values.FirstOrDefault(k => k.Name == name);

        public static HashSet<string> AvailableGrants { get; set; } = new(StringComparer.InvariantCultureIgnoreCase) { "ALL" };

        public static void AddGrantsToAvailableGrants<T>()
        {
            foreach (var method in typeof(T).GetMethods())
            {
                object[] methodAttributes = Attribute.GetCustomAttributes(method, typeof(RequireGrantAttribute));

                if (methodAttributes.Length > 0)
                {
                    foreach (object attr in methodAttributes)
                    {
                        //Console.WriteLine($"    Attribute: {attr.GetType().Name}");
                        if (attr is RequireGrantAttribute myCustomAttr)
                        {
                            //Console.WriteLine($"      Custom Name: {myCustomAttr.Roles}");
                            foreach (var grant in myCustomAttr.Grants)
                            {
                                //Console.WriteLine($"      Custom role: {role}");
                                AvailableGrants.Add(grant);
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

        public static ValueTask<IUser?> AuthenticateRequestAsync(IRequest request, string apiKey)
        {
            //Console.WriteLine($"request: {request.ToString()}  -- apikey: {apiKey}");

            if (AuthDB.Keys.TryGetValue(apiKey, out var key))
            {
                //Console.WriteLine($"apikey: {apiKey} -- name: {key.Name} -- grants: {string.Join("; ", key.Grants)}");
                //Console.WriteLine($"{key.Grants.Contains("ALL")}");

                return new(new ApiUser(key));
            }

            return default;
        }
    }
}
