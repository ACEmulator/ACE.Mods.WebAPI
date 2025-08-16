namespace ACE.Mods.WebAPI.WebServices
{
    public class SettlementsService
    {
        [RequireRole("housing")]
        [ResourceMethod("")]
        public List<SettlementInfo> GetSettlements()
        {
            if (SettlementsDB.Count == 0)
            {
                LoadSettlementDB();
            }

            return SettlementsDB.Values.ToList();
        }

        public static Dictionary<string, SettlementInfo> SettlementsDB = new Dictionary<string, SettlementInfo>();
        public static Dictionary<uint, string> SettlementLandblockIdtoName = new Dictionary<uint, string>();

        private static readonly string settlementDatafileName = "settlementsData.json";
        private static readonly string settlementDatafilePath = Mod.Instance.ModPath + "/data/" + settlementDatafileName;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
        };

        public static void LoadSettlementDB()
        {
            if (File.Exists(settlementDatafilePath))
            {
                // Try loading the keys from file
                using (FileStream fs = File.OpenRead(settlementDatafilePath))
                {
                    SettlementsDB = JsonSerializer.Deserialize<List<SettlementInfo>>(fs, jsonSerializerOptions)?.ToDictionary(k => k.Name ?? "") ?? new();
                }
                //var i = 1;
                foreach (var settlement in SettlementsDB)
                {
                    //settlement.Value.Id = i++;
                    if (settlement.Value.LandblockIds != null)
                    {
                        foreach (var landblock in settlement.Value.LandblockIds)
                        {
                            if (!SettlementLandblockIdtoName.TryAdd(landblock, settlement.Key))
                                Console.WriteLine($"duplicate settlement landblock id: 0x{landblock:X4} | name: {settlement.Key}");
                        }
                    }
                }
            }
            //Debugger.Break();
        }

        public class SettlementInfo
        {
            //public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = "";

            [JsonPropertyName("hubObjCellId")]
            public uint? PortalHubArea { get; set; }

            [JsonPropertyName("portalWcid")]
            public uint? DirectPortalWeenieClassId { get; set; }

            [JsonPropertyName("childLandblocks")]
            public HashSet<uint>? LandblockIds { get; set; }

            [JsonPropertyName("children")]
            public HashSet<uint>? HouseWeenieClassIds { get; set; }

            [JsonPropertyName("signWcid")]
            public uint? SignWeenieClassId { get; set; }
        }

        public class SettlementStub
        {
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = "";

            [JsonPropertyName("childLandblocks")]
            public HashSet<uint>? LandblockIds { get; set; }
        }
    }
}
