namespace ACE.Mods.WebAPI.Controllers
{
    public class StatusController
    {

        public ServerStatusDTO Index()
        {
            var worldName = ConfigManager.Config.Server.WorldName;
            var isOpen = WorldManager.WorldStatus == WorldManager.WorldStatusState.Open;
            var currentConnections = PlayerManager.GetOnlineCount();
            var maxConnections = (int)ConfigManager.Config.Server.Network.MaximumAllowedSessions;

            return new() {
                WorldName = worldName,
                IsOpen = isOpen,
                CurrentConnections = currentConnections,
                MaxConnections = maxConnections
            };
        }

        public class ServerStatusDTO
        {
            public string? WorldName { get; set; }
            public bool IsOpen { get; set; }
            public int CurrentConnections { get; set; }
            public int MaxConnections { get; set; }
        }
    }
}
