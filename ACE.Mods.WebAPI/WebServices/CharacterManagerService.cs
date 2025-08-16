namespace ACE.Mods.WebAPI.WebServices
{
    public class CharacterManagerService
    {
        //[RequireRole("characters")]
        //[ResourceMethod("")]
        //public List<CharacterStubDTO> GetCharactersList()
        //{
        //    using (var context = new ShardDbContext())
        //    {
        //        var results = context.Character
        //            .AsNoTracking();
        //        //.Where(r => r.AccessLevel == Convert.ToUInt32(accessLevel)).ToList();

        //        var result = new List<CharacterStubDTO>();
        //        foreach (var character in results)
        //            result.Add(new() { Id = character.Id, Name = character.Name, IsDeleted = character.IsDeleted });

        //        return result;
        //    }
        //}

        [RequireRole("characters")]
        [ResourceMethod("")]
        public List<CharacterStubDTO> GetCharacters(uint? accountId)
        {
            return (GetCharactersList(accountId));
        }

        public static List<CharacterStubDTO> GetCharactersList(uint? accountId)
        {
            using (var context = new ShardDbContext())
            {
                List<Character>? results;

                if (accountId != null)
                {
                    results = context.Character
                                     .AsNoTracking()
                                     .Where(c => c.AccountId == accountId).ToList();
                }
                else
                {
                    results = context.Character
                                     .AsNoTracking().ToList();
                }

                var result = new List<CharacterStubDTO>();
                foreach (var character in results)
                    result.Add(new() { Id = character.Id, Name = character.Name, IsDeleted = character.IsDeleted });

                return result;
            }
        }

        [RequireRole("characters")]
        [ResourceMethod(":characterId")]
        public CharacterDTO? GetCharacter(uint characterId)
        {
            return GetCharacterDTO(characterId);
        }

        public static CharacterDTO? GetCharacterDTO(uint characterId)
        {
            var character = DatabaseManager.Shard.BaseDatabase.GetCharacter(characterId);

            //DatabaseManager.Shard.GetCharacter(characterId, static character =>
            //{
            //    if (character == null)
            //        return null;
            //});

            if (character == null)
                return null;

            return PackCharacter(character);
        }

        public static CharacterDTO? GetAccountCharacterDTO(uint accountId, uint characterId)
        {
            var characters = DatabaseManager.Shard.BaseDatabase.GetCharacters(accountId, true);

            //DatabaseManager.Shard.GetCharacter(characterId, static character =>
            //{
            //    if (character == null)
            //        return null;
            //});

            if (characters == null)
                return null;

            var character = characters.Where(c => c.Id == characterId).FirstOrDefault();

            if (character == null)
                return null;

            return PackCharacter(character);
        }

        private static CharacterDTO PackCharacter(Character character)
        {
            var characterToReturn = new CharacterDTO();
            characterToReturn.Id = character.Id;
            characterToReturn.AccountId = character.AccountId;
            characterToReturn.Name = character.Name;
            characterToReturn.IsPlussed = character.IsPlussed;
            characterToReturn.IsDeleted = character.IsDeleted;
            characterToReturn.DeleteTime = character.DeleteTime;
            characterToReturn.LastLoginTimestamp = character.LastLoginTimestamp;
            characterToReturn.TotalLogins = character.TotalLogins;
            characterToReturn.CharacterOptions1 = character.CharacterOptions1;
            characterToReturn.CharacterOptions2 = character.CharacterOptions2;
            characterToReturn.GameplayOptions = character.GameplayOptions;
            characterToReturn.SpellbookFilters = character.SpellbookFilters;
            characterToReturn.HairTexture = character.HairTexture;
            characterToReturn.DefaultHairTexture = character.DefaultHairTexture;

            return characterToReturn;
        }

        public class CharacterStubDTO
        {
            public uint Id { get; set; }
            public string? Name { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class CharacterDTO
        {
            public uint Id { get; set; }
            public uint AccountId { get; set; }
            public string? Name { get; set; }
            public bool IsPlussed { get; set; }
            public bool IsDeleted { get; set; }
            public ulong? DeleteTime { get; set; }
            public double? LastLoginTimestamp { get; set; }
            public int TotalLogins { get; set; }
            public int CharacterOptions1 { get; set; }
            public int CharacterOptions2 { get; set; }
            public byte[]? GameplayOptions { get; set; }
            public uint SpellbookFilters { get; set; }
            public uint HairTexture { get; set; }
            public uint DefaultHairTexture { get; set; }
        }
    }
}
