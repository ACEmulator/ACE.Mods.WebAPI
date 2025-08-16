namespace ACE.Mods.WebAPI.WebServices
{
    public class AccountManagerService
    {
        [RequireRole("accounts")]
        [ResourceMethod("")]
        public List<AccountStubDTO> GetAccounts()
        {
            using (var context = new AuthDbContext())
            {
                var results = context.Account
                    .AsNoTracking();
                    //.Where(r => r.AccessLevel == Convert.ToUInt32(accessLevel)).ToList();

                var result = new List<AccountStubDTO>();
                foreach (var account in results)
                    result.Add(new() { Id = account.AccountId, Name = account.AccountName });

                return result;
            }
        }

        [RequireRole("accounts")]
        [ResourceMethod(":accountName")]
        public AccountDTO? GetAccount(string accountName)
        {
            var account = DatabaseManager.Authentication.GetAccountByName(accountName);

            if (account == null)
                return null;

            return PackAccount(account);
        }

        //[RequireRole("accounts")]
        //[ResourceMethod(":accountId")]
        //public AccountDTO? GetAccount(uint accountId)
        //{
        //    var account = DatabaseManager.Authentication.GetAccountById(accountId);

        //    if (account == null)
        //        return null;

        //    return PackAccount(account);
        //}

        [RequireRole("accounts")]
        [ResourceMethod(":accountName/characters")]
        public List<CharacterManagerService.CharacterStubDTO>? GetCharacters(string accountName)
        {
            var account = DatabaseManager.Authentication.GetAccountByName(accountName);

            if (account == null)
                return null;

            return CharacterManagerService.GetCharactersList(account.AccountId);
        }

        [RequireRole("accounts")]
        [ResourceMethod(":accountName/characters/:characterId")]
        public CharacterManagerService.CharacterDTO? GetCharacter(string accountName, uint characterId)
        {
            var account = DatabaseManager.Authentication.GetAccountByName(accountName);

            if (account == null)
                return null;

            return CharacterManagerService.GetAccountCharacterDTO(account.AccountId, characterId);
        }

        private static AccountDTO PackAccount(Account account)
        {
            var accountToReturn = new AccountDTO();
            accountToReturn.Id = account.AccountId;
            accountToReturn.Name = account.AccountName;
            accountToReturn.AccessLevel = account.AccessLevel;
            accountToReturn.EmailAddress = account.EmailAddress;
            accountToReturn.CreationTime = account.CreateTime;
            if (account.CreateIP != null)
            {
                var createIP = new IPAddress(account.CreateIP);
                accountToReturn.CreationIP = createIP.ToString();
            }
            accountToReturn.LastLoginTime = account.LastLoginTime;
            if (account.LastLoginIP != null)
            {
                var lastIP = new IPAddress(account.LastLoginIP);
                accountToReturn.LastLoginIP = lastIP.ToString();
            }
            accountToReturn.TotalTimesLoggedIn = account.TotalTimesLoggedIn;
            accountToReturn.BannedTime = account.BannedTime;
            accountToReturn.BanExpirationTime = account.BanExpireTime;
            accountToReturn.BanReason = account.BanReason;
            accountToReturn.BannedByAccountId = account.BannedByAccountId;

            return accountToReturn;
        }

        public class AccountStubDTO
        {
            public uint Id { get; set; }
            public string? Name { get; set; }
        }

        public class AccountDTO
        {
            public uint Id { get; set; }
            public string? Name { get; set; }
            public uint AccessLevel { get; set; }
            public string? EmailAddress { get; set; }
            public DateTime? CreationTime { get; set; }
            public string? CreationIP { get; set; }
            public DateTime? LastLoginTime { get; set; }
            public string? LastLoginIP { get; set; }
            public uint? TotalTimesLoggedIn { get; set; }
            public DateTime? BannedTime { get; set; }
            public DateTime? BanExpirationTime { get; set; }
            public string? BanReason { get; set; }
            public uint? BannedByAccountId { get; set; }
        }
    }
}
