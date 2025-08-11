namespace ACE.Mods.WebAPI;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    private static IServerHost? serverHost;

    public override void Init()
    {
        base.Init();
    }

    public override Task OnStartSuccess()
    {
        Settings = SettingsContainer?.Settings ?? new();
        StartServices();

        return Task.CompletedTask;
    }

    //public override Task OnWorldOpen()
    //{
    //    Settings = SettingsContainer?.Settings ?? new();
    //    StartServices();

    //    return Task.CompletedTask;
    //}

    protected override void SettingsChanged(object? sender, EventArgs e)
    {
        StopServices();
        base.SettingsChanged(sender, e);
        Settings = SettingsContainer?.Settings ?? new();
        StartServices();
    }

    public override void Stop()
    {
        StopServices();
        base.Stop();
    }

    public static void StartServices()
    {
        try
        {
            //var content = Content.From(Resource.FromString("Hello World!"));

            //var server = Host.Create()
            //                 .Handler(content)
            //                 .Defaults()
            //                 .StartAsync(); // or .RunAsync() to block until the application is shut down
            //                 //.RunAsync();

            serverHost = Host.Create();

            //var app = Layout.Create();
                            //.AddService<BookService>("books")
                            //.AddController<IotController>("device")
                            //.AddOpenApi()
                            //.AddSwaggerUI()
                            //.AddRedoc()
                            //.AddScalar();

            var api = Layout.Create();
            var secure_api = Layout.Create();

            var auth = ApiKeyAuthentication.Create()
                                           //.WithQueryParameter("apiKey")
                                           .WithHeader("X-API-Key")
                                           .Authenticator(AuthenticateRequestAsync);

            //api.Add(auth);

            api.AddController<StatusController>("status");

            //api.AddService<DerethPulseService>("derethpulse");
            //api.Add("derethpulse", ServiceResource.From<DerethPulseService>().Authentication(auth));
            //api.AddController<EventManagerController>("events2");
            secure_api.AddService<DerethPulseService>("derethpulse");
            APIKeys.AddRolesToAvailableGrants<DerethPulseService>();

            //api.AddService<EventManagerService>("events");
            //api.Add("events", ServiceResource.From<EventManagerService>().Authentication(auth));
            secure_api.AddService<EventManagerService>("events");
            APIKeys.AddRolesToAvailableGrants<EventManagerService>();

            //api.AddService<AccountManagerService>("accounts");
            //api.AddService<CharacterManagerService>("characters");
            //api.AddService<PlayerManagerService>("players");

            //api.AddService<AllegianceManagerService>("allegiances");

            secure_api.AddService<AccountManagerService>("accounts");
            APIKeys.AddRolesToAvailableGrants<AccountManagerService>();
            secure_api.AddService<CharacterManagerService>("characters");
            APIKeys.AddRolesToAvailableGrants<CharacterManagerService>();
            secure_api.AddService<PlayerManagerService>("players");
            APIKeys.AddRolesToAvailableGrants<PlayerManagerService>();

            secure_api.AddService<AllegianceManagerService>("allegiances");
            APIKeys.AddRolesToAvailableGrants<AllegianceManagerService>();

            var description = ApiDescription.Create()
                                .Title(Mod.Instance.Container.Meta.Name)
                                .Version(Mod.Instance.Container.Meta.Version)
                                .PostProcessor((r, doc) =>
                                {
                                    doc.Servers.Clear();
                                    doc.Servers.Add(new NSwag.OpenApiServer() { Url = Settings.APIBaseUrl + "/" + Settings.APIBasePath });
                                    doc.SecurityDefinitions.Add("X-API-Key", new NSwag.OpenApiSecurityScheme()
                                    {
                                        //Name = "Baz",
                                        //Description = "Bar",
                                        //Type = OpenApiSecuritySchemeType.Basic,
                                        //Flow = OpenApiOAuth2Flow.Application,
                                        //In = OpenApiSecurityApiKeyLocation.Header,
                                        //AuthorizationUrl = "AuthUrl",
                                        Name = "X-API-Key",
                                        //Description = "Bar",
                                        Type = OpenApiSecuritySchemeType.ApiKey,
                                        In = OpenApiSecurityApiKeyLocation.Header,
                                    });
                                    //IEnumerable<string> emptyStringList = new List<string> { };
                                    var emptyStringList = new List<string> { };
                                    var apiKeySecurityRequirement = new OpenApiSecurityRequirement();
                                    apiKeySecurityRequirement.Add("X-API-Key", emptyStringList);

                                    foreach (var path in doc.Paths)
                                    {
                                        if (path.Key == "/status/")
                                            continue;

                                        foreach (var pathValue in path.Value.Values)
                                        {
                                            var securityRequirementList = new List<OpenApiSecurityRequirement>();
                                            pathValue.Security ??= securityRequirementList;
                                            pathValue.Security.Add(apiKeySecurityRequirement);

                                            pathValue.Responses.Add("401", new OpenApiResponse() { Description = "Unauthorized" });
                                        }
                                    }
                                });
                                //.PostProcessor((r, doc) => doc.Info.TermsOfService = "https://mycompany.com/tos");

            if (Settings.EnableOpenAPI)
                api.AddOpenApi().Add(description);

            if (Settings.EnableSwaggerUI)
                api.AddSwaggerUI();

            if (Settings.EnableRedoc)
                api.AddRedoc();

            if (Settings.EnableScalar)
                api.AddScalar();

            //var auth = BasicAuthentication.Create()
            //                              .Add("Bob", "pw123");

            //var auth = ApiKeyAuthentication.Create()
            //                               //.WithQueryParameter("apiKey")
            //                               .WithHeader("X-API-Key")
            //                               .Authenticator(AuthenticateRequestAsync);

            //api.Add(auth);

            secure_api.Add(auth);

            api.Add(secure_api);

            var app = Layout.Create().Add(Settings.APIBasePath, api);

            //app.Add(auth);

            serverHost?.Handler(app);
                       //.Defaults()
                       //.Development()
                       //.Console()
                       //.StartAsync();

            //serverHost?.Add(auth);

            serverHost?.Defaults();

            var host = IPAddress.Parse(Settings.Host);
            var port = Settings.Port;

            serverHost?.Bind(host, port);

            if (Settings.OutputToConsole)
                serverHost?.Console();

            var server = serverHost?.StartAsync();

            Mod.Log($"API Server Online and listening to requests at http://{host}:{port}");

            APIKeys.Load();
            Mod.Log($"API Server has loaded and activated {APIKeys.Keys.Count} keys from storage");
        }
        catch (Exception ex)
        {
            Mod.Log($"ERROR during initialization - {ex.Message}", ModManager.LogLevel.Error);
        }
    }

    public static void StopServices()
    {
        serverHost?.StopAsync();

        serverHost = null;

        APIKeys.Keys.Clear();

        Mod.Log("API Server Offline");
    }

    static ValueTask<IUser?> AuthenticateRequestAsync(IRequest request, string apiKey)
    {
        //Console.WriteLine($"request: {request.ToString()}  -- apikey: {apiKey}");

        //if (apiKey == "abc")
        //{
        //    return new(new ApiKeyUser(apiKey, "ADMIN", "USER"));
        //}

        //if (apiKey == "bcd")
        //{
        //    return new(new ApiKeyUser(apiKey, "USER"));
        //}

        //return new(new ApiKeyUser(apiKey, "ADMIN"));

        //return new(new ApiKeyUser(apiKey));

        if (APIKeys.Keys.TryGetValue(apiKey, out var key))
        {
            //Console.WriteLine($"apikey: {apiKey} -- name: {key.Name} -- grants: {string.Join("; ", key.Grants)}");
            //Console.WriteLine($"{key.Grants.Contains("ALL")}");

            if (key.Grants.Contains("ALL"))
                return new(new ApiKeyUser(apiKey, APIKeys.AvailableGrants.ToArray()));

            return new(new ApiKeyUser(apiKey, key.Grants.ToArray()));
        }

        return default;
    }
}

