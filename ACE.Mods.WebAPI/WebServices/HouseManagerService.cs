namespace ACE.Mods.WebAPI.WebServices
{
    public class HouseManagerService
    {
        [RequireRole("housing")]
        [ResourceMethod("")]
        public List<HouseStubDTO> GetHousing(int? houseType, bool includeApartments = false)
        {
            var housesToReturn = new List<HouseStubDTO>();

            if (HUD.Count == 0)
            {
                BuildHouses();
            }

            for (var i = 1u; i < 6251; i++)
            //for (var i = 1u; i < 25; i++)
            {
                if (HUD.TryGetValue(i, out var houseDatabaseCollection))
                {
                    if (houseType == null && !includeApartments && houseDatabaseCollection.HouseType == (int)HouseType.Apartment)
                        continue;

                    if (houseType != null && houseDatabaseCollection.HouseType != houseType)
                        continue;

                    var houseToReturn = new HouseStubDTO()
                    {
                        Id = i,
                        Type = houseDatabaseCollection.HouseType,
                        LOC = houseDatabaseCollection.Location.ToLOCString(),
                    };

                    if (houseToReturn.Type == (int)HouseType.Apartment)
                    {
                        if (HouseManager.ApartmentBlocks.TryGetValue((uint)houseDatabaseCollection.Location.LandblockId.Landblock, out var complexName))
                            houseToReturn.Settlement = complexName;
                    }
                    else
                    {
                    }

                    var houseOwned = HouseManager.GetHouseById(i).FirstOrDefault();

                    if (houseOwned != null)
                    {
                        houseToReturn.IsOwned = true;
                        houseToReturn.IsRentPaid = houseOwned.SlumLord.IsRentPaid();
                    }

                    housesToReturn.Add(houseToReturn);
                }
            }

            return housesToReturn;
        }

        private static Dictionary<uint, HouseDatabaseCollection> HUD { get; set; } = new();

        private static void BuildHouses()
        {
            using (var ctx = new WorldDbContext())
            {
                var query = from slumlord_weenie in ctx.Weenie where slumlord_weenie.Type == (int)WeenieType.SlumLord
                            join slumlord_instance in ctx.LandblockInstance on slumlord_weenie.ClassId equals slumlord_instance.WeenieClassId
                            join parentlink in ctx.LandblockInstanceLink on slumlord_instance.Guid equals parentlink.ChildGuid
                            join house_instance in ctx.LandblockInstance on parentlink.ParentGuid equals house_instance.Guid
                            join house_weenie in ctx.Weenie on house_instance.WeenieClassId equals house_weenie.ClassId
                            join house_weenie_house_id in ctx.WeeniePropertiesDID on house_weenie.ClassId equals house_weenie_house_id.ObjectId where house_weenie_house_id.Type == (int)PropertyDataId.HouseId
                            join house_weenie_house_type in ctx.WeeniePropertiesInt on house_weenie.ClassId equals house_weenie_house_type.ObjectId where house_weenie_house_type.Type == (int)PropertyInt.HouseType
                            select new
                            {
                                HouseId = house_weenie_house_id.Value,
                                HouseType = house_weenie_house_type.Value,
                                House = house_weenie,
                                HouseInstance = house_instance,
                                SlumLord = slumlord_weenie,
                                SlumLordInstance = slumlord_instance,
                            };

                //var houses = query.OrderBy(q => q.HouseId).ToList();
                var houses = query.ToList();

                //Debugger.Break();

                foreach (var house in houses)
                {
                    if (!HUD.TryAdd(house.HouseId, new()
                    {
                        HouseId = house.HouseId,
                        HouseType = house.HouseType,
                        House = house.House,
                        HouseInstance = house.HouseInstance,
                        SlumLord = house.SlumLord,
                        SlumLordInstance = house.SlumLordInstance
                    }))
                        Console.WriteLine($"duplicate house id {house.HouseId} | 0x{house.HouseInstance.Guid:X8}");
                }
            }
        }

        public class HouseDatabaseCollection
        {
            public uint HouseId { get; set; }
            public int HouseType { get; set; }
            public Database.Models.World.Weenie? House { get; set; }
            public LandblockInstance? HouseInstance { get; set; }
            public Database.Models.World.Weenie? SlumLord { get; set; }
            public LandblockInstance? SlumLordInstance { get; set; }

            private Position? _location;
            public Position Location => _location ??= new Position(HouseInstance?.ObjCellId ?? 0,
                        HouseInstance?.OriginX ?? 0, HouseInstance?.OriginY ?? 0, HouseInstance?.OriginZ ?? 0,
                        HouseInstance?.AnglesX ?? 0, HouseInstance?.AnglesY ?? 0, HouseInstance?.AnglesZ ?? 0, HouseInstance?.AnglesW ?? 0);
        }

        public class HouseStubDTO
        {
            public uint Id { get; set; }
            //public uint Guid { get; set; }
            public int Type { get; set; }
            //public string? Name { get; set; }
            public string? LOC { get; set; }
            public bool IsOwned { get; set; }
            public bool? IsRentPaid { get; set; }

            public string? Settlement { get; set; }
        }
    }
}
