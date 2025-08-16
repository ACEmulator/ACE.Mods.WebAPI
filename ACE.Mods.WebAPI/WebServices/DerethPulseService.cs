namespace ACE.Mods.WebAPI.WebServices
{
    public class DerethPulseService
    {

        [RequireRole("derethpulse")]
        [ResourceMethod("players")]
        public List<PlayerData> GetPlayers()
        {
            var playerData = new List<PlayerData>();

            var players = PlayerManager.GetAllOnline();

            foreach (var player in players)
            {
                var radarData = ExtractPlayerData(player);
                if (radarData != null)
                {
                    playerData.Add(radarData);
                }
            }

            return playerData;
        }

        private PlayerData? ExtractPlayerData(Player player)
        {
            try
            {
                // Get player name
                var playerName = player.Name ?? "Unknown";

                // Get player race from HeritageGroup
                var heritage = player.HeritageGroup;
                var race = heritage switch
                {
                    HeritageGroup.Aluvian => "Aluvian",
                    HeritageGroup.Sho => "Sho",
                    HeritageGroup.Gharundim => "Gharu'ndim",
                    HeritageGroup.Viamontian => "Viamontian",
                    HeritageGroup.Shadowbound => "Shadowbound",
                    HeritageGroup.Gearknight => "Gearknight",
                    HeritageGroup.Tumerok => "Tumerok",
                    HeritageGroup.Lugian => "Lugian",
                    HeritageGroup.Empyrean => "Empyrean",
                    HeritageGroup.Penumbraen => "Penumbraen",
                    HeritageGroup.Undead => "Undead",
                    HeritageGroup.Olthoi => "Olthoi",
                    HeritageGroup.OlthoiAcid => "Olthoi",
                    _ => "Unknown"
                };

                // Get player level
                var level = player.Level?.ToString() ?? "1";

                // Get map coordinates
                var mapCoords = player.Location.GetMapCoords();
                if (!mapCoords.HasValue)
                {
                    // Player is indoors or in a dungeon - skip
                    return null;
                }

                // Format coordinates as "16.9E", "22.9S" style
                var x = mapCoords.Value.X >= 0 ? $"{mapCoords.Value.X:F1}E" : $"{Math.Abs(mapCoords.Value.X):F1}W";
                var y = mapCoords.Value.Y >= 0 ? $"{mapCoords.Value.Y:F1}N" : $"{Math.Abs(mapCoords.Value.Y):F1}S";

                // Get LOC data for better precision
                var loc = player.Location.ToLOCString();

                return new PlayerData
                {
                    //Type = "Player",
                    LocationName = playerName,
                    Race = race,
                    Level = level,
                    X = x,
                    Y = y,
                    LOC = loc
                };
            }
            catch (Exception ex)
            {
                if (PatchClass.Settings?.EnableLogging == true)
                    Mod.Log($"Error extracting data for player {player.Name} - {ex.Message}", ModManager.LogLevel.Warn);
                return null;
            }
        }

        [RequireRole("derethpulse")]
        [ResourceMethod("landblocks")]
        public List<LandblockData> GetLandblocks()
        {
            var landblockData = new List<LandblockData>();

            var landblocks = LandblockManager.GetLoadedLandblocks();

            foreach (var landblock in landblocks)
            {
                var radarData = ExtractLandblockData(landblock);
                if (radarData != null)
                {
                    landblockData.Add(radarData);
                }
            }

            return landblockData;
        }

        private LandblockData? ExtractLandblockData(Landblock landblock)
        {
            try
            {
                // Get landblock id
                var id = "0x" + landblock.Id.ToString()[0..4];

                // Get landblock status
                var status = "Active";
                if (landblock.Permaload)
                    status = "Permaload";
                else if (landblock.IsDormant)
                    status = "Dormant";

                var hasKeepAliveObjects = !landblock.HasNoKeepAliveObjects;

                if (hasKeepAliveObjects)
                    status = "KeepAlive";

                // Get Landblock X / Y
                var x = landblock.Id.LandblockX.ToString("x2");
                var y = landblock.Id.LandblockY.ToString("x2");

                // Get Landblock dungeon status
                var isDungeon = landblock.IsDungeon;
                var hasDungeon = landblock.HasDungeon;

                // Get Landblock Player and Creature counts
                var playerCount = landblock.GetPlayers().Count;
                var creatureCount = landblock.GetCreatures().Count;

                return new LandblockData
                {
                    Id = id,
                    Status = status,
                    HasKeepAliveObjects = hasKeepAliveObjects,
                    X = x,
                    Y = y,
                    IsDungeon = isDungeon,
                    HasDungeon = hasDungeon,
                    PlayerCount = playerCount,
                    CreatureCount = creatureCount,
                };
            }
            catch (Exception ex)
            {
                if (PatchClass.Settings?.EnableLogging == true)
                    Mod.Log($"Error extracting data for landblock {landblock.Id} - {ex.Message}", ModManager.LogLevel.Warn);
                return null;
            }
        }

        public class PlayerData
        {
            [JsonPropertyName("Type")]
            public string Type { get; set; } = "Player";

            [JsonPropertyName("LocationName")]
            public string LocationName { get; set; } = "";

            [JsonPropertyName("Race")]
            public string Race { get; set; } = "";

            [JsonPropertyName("Level")]
            public string Level { get; set; } = "";

            [JsonPropertyName("x")]
            public string X { get; set; } = "";

            [JsonPropertyName("y")]
            public string Y { get; set; } = "";

            [JsonPropertyName("loc")]
            public string LOC { get; set; } = "";
        }

        public class LandblockData
        {
            [JsonPropertyName("Type")]
            public string Type { get; set; } = "Landblock";

            [JsonPropertyName("id")]
            public string Id { get; set; } = "";

            [JsonPropertyName("Status")]
            public string Status { get; set; } = "";

            [JsonPropertyName("hasKeepAliveObjects")]
            public bool HasKeepAliveObjects { get; set; } = false;

            [JsonPropertyName("x")]
            public string X { get; set; } = "";

            [JsonPropertyName("y")]
            public string Y { get; set; } = "";

            [JsonPropertyName("isDungeon")]
            public bool IsDungeon { get; set; } = false;

            [JsonPropertyName("hasDungeon")]
            public bool HasDungeon { get; set; } = false;

            [JsonPropertyName("playerCount")]
            public int PlayerCount { get; set; } = 0;

            [JsonPropertyName("creatureCount")]
            public int CreatureCount { get; set; } = 0;
        }
    }
}
