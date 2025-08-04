namespace ACE.Mods.WebAPI;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    //private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    //{
    //    WriteIndented = true,
    //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //};

    private IServerHost? serverHost;

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

    public void StartServices()
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

            api.AddController<StatusController>("status");
            api.AddService<DerethPulseService>("derethpulse");
            //api.AddController<EventManagerController>("events2");
            api.AddService<EventManagerService>("events");

            var description = ApiDescription.Create()
                                .Title(Mod.Instance.Container.Meta.Name)
                                .Version(Mod.Instance.Container.Meta.Version)
                                .PostProcessor((r, doc) =>
                                {
                                    doc.Servers.Clear();
                                    doc.Servers.Add(new NSwag.OpenApiServer() { Url = Settings.APIBaseUrl + "/" + Settings.APIBasePath });
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

            var auth = ApiKeyAuthentication.Create()
                                           //.WithQueryParameter("apiKey")
                                           .WithHeader("X-API-Key")
                                           .Authenticator(AuthenticateRequestAsync);

            //api.Add(auth);

            var app = Layout.Create().Add(Settings.APIBasePath, api);

            serverHost?.Handler(app);
                       //.Defaults()
                       //.Development()
                       //.Console()
                       //.StartAsync();

            serverHost?.Defaults();

            var host = System.Net.IPAddress.Parse(Settings.Host);
            var port = Settings.Port;

            serverHost?.Bind(host, port);

            if (Settings.OutputToConsole)
                serverHost?.Console();

            var server = serverHost?.StartAsync();

            Mod.Log($"API Server Online and listening to requests at http://{host}:{port}");

        }
        catch (Exception ex)
        {
            Mod.Log($"ERROR during initialization - {ex.Message}", ModManager.LogLevel.Error);
        }
    }

    public void StopServices()
    {
        serverHost?.StopAsync();

        serverHost = null;

        Mod.Log("API Server Offline");
    }

    static ValueTask<IUser?> AuthenticateRequestAsync(IRequest request, string apiKey)
    {
        //if (apiKey == "abc")
        //{
        //    return new(new ApiKeyUser(apiKey, "ADMIN", "USER"));
        //}

        //if (apiKey == "bcd")
        //{
        //    return new(new ApiKeyUser(apiKey, "USER"));
        //}

        return default;
    }
}

