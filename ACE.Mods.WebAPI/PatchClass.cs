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

    //protected override void SettingsChanged(object? sender, EventArgs e)
    //{
    //    StopServices();
    //    base.SettingsChanged(sender, e);
    //    Settings = SettingsContainer?.Settings ?? new();
    //    StartServices();
    //}

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

            serverHost = Host.Create();

            var api = Layout.Create();
            var secure_api = Layout.Create();

            api.AddController<StatusController>("status");

            secure_api.AddService<DerethPulseService>("derethpulse");
            AuthDB.AddGrantsToAvailableGrants<DerethPulseService>();

            secure_api.AddService<EventManagerService>("events");
            AuthDB.AddGrantsToAvailableGrants<EventManagerService>();

            secure_api.AddService<HouseManagerService>("housing");
            AuthDB.AddGrantsToAvailableGrants<HouseManagerService>();

            secure_api.AddService<AccountManagerService>("accounts");
            AuthDB.AddGrantsToAvailableGrants<AccountManagerService>();
            secure_api.AddService<CharacterManagerService>("characters");
            AuthDB.AddGrantsToAvailableGrants<CharacterManagerService>();
            secure_api.AddService<PlayerManagerService>("players");
            AuthDB.AddGrantsToAvailableGrants<PlayerManagerService>();

            secure_api.AddService<AllegianceManagerService>("allegiances");
            AuthDB.AddGrantsToAvailableGrants<AllegianceManagerService>();

            var description = ApiDescription.Create()
                                .Title(Mod.Instance.Container.Meta.Name)
                                .Version(Mod.Instance.Container.Meta.Version)
                                .PostProcessor((r, doc) =>
                                {
                                    doc.Servers.Clear();
                                    doc.Servers.Add(new NSwag.OpenApiServer() { Url = Settings.APIBaseUrl + "/" + Settings.APIBasePath });
                                    doc.SecurityDefinitions.Add("X-API-Key", new NSwag.OpenApiSecurityScheme()
                                    {
                                        Name = "X-API-Key",
                                        //Description = "X-API-Key",
                                        Type = OpenApiSecuritySchemeType.ApiKey,
                                        In = OpenApiSecurityApiKeyLocation.Header,
                                    });
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
                                            pathValue.Responses.Add("403", new OpenApiResponse() { Description = "Forbidden" });
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

            var auth = ApiKeyAuthentication.Create()
                                           //.WithQueryParameter("apiKey")
                                           .WithHeader("X-API-Key")
                                           .Authenticator(AuthDB.AuthenticateRequestAsync);

            secure_api.Add(auth);

            api.Add(secure_api);

            var app = Layout.Create().Add(Settings.APIBasePath, api);

            app.Add(CorsPolicy.Permissive());

            serverHost?.Handler(app);
                       //.Defaults()
                       //.Development()
                       //.Console()
                       //.StartAsync();

            serverHost?.Defaults();

            var host = IPAddress.Parse(Settings.Host);
            var port = Settings.Port;

            serverHost?.Bind(host, port);

            if (Settings.OutputToConsole)
                serverHost?.Console();

            var server = serverHost?.StartAsync();

            Mod.Log($"API Server Online and listening to requests at:\n http://{host}:{port}" + $"/{Settings.APIBasePath}");
            if (Settings.EnableSwaggerUI)
                Mod.Log($"SwaggerUI API Browser available at:\n http://{host}:{port}" + $"/{Settings.APIBasePath}/swagger/");
            if (Settings.EnableRedoc)
                Mod.Log($"Redoc API Browser available at:\n\t http://{host}:{port}" + $"/{Settings.APIBasePath}/redoc/");
            if (Settings.EnableScalar)
                Mod.Log($"Scalar API Browser available at:\n http://{host}:{port}" + $"/{Settings.APIBasePath}/scalar/");

            AuthDB.Load();
            Mod.Log($"API Server has loaded and activated {AuthDB.Keys.Count} keys from storage");
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

        AuthDB.Keys.Clear();

        Mod.Log("API Server Offline");
    }
}
