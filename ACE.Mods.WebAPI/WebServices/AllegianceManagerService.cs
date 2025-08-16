namespace ACE.Mods.WebAPI.WebServices
{
    public class AllegianceManagerService
    {
        [RequireRole("allegiances")]
        [ResourceMethod("")]
        public List<AllegianceDTO> GetAllegiances()
        {
            var allegiancesToReturn = new List<AllegianceDTO>();

            var allegiances = AllegianceManager.Allegiances;

            foreach (var allegiance in allegiances.Values.OrderBy(a => a.Guid.Full))
            {
                var allegianceDTO = new AllegianceDTO();

                allegianceDTO.Guid = allegiance.Guid.Full;
                allegianceDTO.Name = allegiance.Name;

                allegianceDTO.MonarchGuid = allegiance.Monarch.Player.Guid.Full;
                allegianceDTO.MonarchName = allegiance.Monarch.Player.Name;

                allegiancesToReturn.Add(allegianceDTO);
            }

            return allegiancesToReturn;
        }

        public class AllegianceDTO
        {
            public uint Guid { get; set; }
            public string? Name { get; set; }

            public uint MonarchGuid { get; set; }
            public string? MonarchName { get; set; }
        }
    }
}
