namespace ACE.Mods.WebAPI.WebServices
{
    public class PlayerManagerService
    {
        [RequireGrant("players")]
        [ResourceMethod("")]
        public List<PlayerDTO> GetPlayers()
        {
            var playersToReturn = new List<PlayerDTO>();

            var players = PlayerManager.GetAllPlayers();

            foreach (var player in players.OrderBy(p => p.Guid.Full))
            {
                var playerDTO = new PlayerDTO();

                playerDTO.Guid = player.Guid.Full;
                playerDTO.Name = player.Name;

                playersToReturn.Add(playerDTO);
            }

            return playersToReturn;
        }

        public class PlayerDTO
        {
            public uint Guid { get; set; }
            public string? Name { get; set; }
        }
    }
}
