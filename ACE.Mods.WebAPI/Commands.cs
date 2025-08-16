namespace ACE.Mods.WebAPI
{
    public class Commands
    {
        //private const string BYE_COMMAND = "bye";

        ////ACE-style command using attribute
        //[CommandHandler(BYE_COMMAND, AccessLevel.Player, CommandHandlerFlag.None, -1, "Manage mods the lazy way")]
        //public static void HandleListMods(Session session, params string[] parameters)
        //{
        //    //If Meta.json has RegisterCommands set to true, ACE will attempt to load/unload automatically
        //    //Mod.Container.RegisterCommandHandlers(true);
        //    Console.WriteLine("Bye.");
        //    //session?.LogOffPlayer(true);
        //}

        [CommandHandler("api", AccessLevel.Admin, CommandHandlerFlag.None, 0, "API management commands")]
        public static void HandleAPIcommand(Session session, params string[] parameters)
        {
            if (parameters.Length == 0 || (parameters.Length >= 1 && parameters[0] == "help"))
            {
                var msg = "@api help: shows this message.\n";
                msg += "@api start: starts the API service.\n";
                msg += "@api stop: stops the API service.\n";
                msg += "@api showkeys: lists all active api keys.\n";
                msg += "@api showkey <name of key>: shows key and grants for specified name.\n";
                msg += "@api showgrants: lists all available grants for keys.\n";
                msg += "@api generatekey <name for key>, <grants>: generates key and saves with specified name and grants.\n";
                msg += "@api modifykey <name of key>, <add | remove> <grants>: modifies grants for key specified by name.\n";
                msg += "@api revokekey <name of key>: deletes key and grants for specified name.\n";

                WriteOutputInfo(session, msg, ChatMessageType.WorldBroadcast);
            }
            else if (parameters[0] == "start")
            {
                PatchClass.StartServices();
            }
            else if (parameters[0] == "stop")
            {
                PatchClass.StopServices();
            }
            else if (parameters[0] == "showkeys")
            {
                var msg = "\n";
                if (APIKeys.Keys.Count > 0)
                {
                    msg += "API Keys\n";
                    msg += "=========================================================\n";
                    foreach (var apiKey in APIKeys.Keys.Values)
                    {
                        msg += $"Name: {apiKey.Name}\n";
                        //msg += $"Key: {apiKey.Key}\n";
                        msg += $"Grants: {string.Join("; ", apiKey.Grants)}\n";
                        msg += "=========================================================\n";
                    }
                }
                else
                {
                    msg += "There are no active API keys.";
                }

                WriteOutputInfo(session, msg, ChatMessageType.WorldBroadcast);
            }
            else if (parameters[0] == "showkey")
            {
                var msg = "\n";
                if (parameters.Length < 2)
                {
                    msg += "You must specify a name for the key to show.";
                }
                else
                {
                    var nameToFind = string.Join(" ", parameters[1..]);

                    var apiKey = APIKeys.GetKeyByName(nameToFind);

                    if (apiKey == null)
                    {
                        msg += $"There is no key named: {nameToFind}";
                    }
                    else
                    {
                        msg += "=========================================================\n";
                        msg += $"Name: {apiKey.Name}\n";
                        msg += $"Key: {apiKey.Key}\n";
                        msg += $"Grants: {string.Join("; ", apiKey.Grants)}\n";
                        msg += "=========================================================\n";
                    }
                }

                WriteOutputInfo(session, msg, ChatMessageType.WorldBroadcast);
            }
            else if (parameters[0] == "showgrants")
            {
                var msg = "\n";
                if (APIKeys.AvailableGrants.Count > 0)
                {

                    msg += "API Grants\n";
                    msg += "=========================================================\n";
                    foreach (var grant in APIKeys.AvailableGrants)
                    {
                        msg += $"{grant}\n";
                    }
                    msg += "=========================================================\n";
                }
                else
                {
                    msg += "There are no available grants.";
                }
                WriteOutputInfo(session, msg, ChatMessageType.WorldBroadcast);
            }
            else if (parameters[0] == "generatekey")
            {
                var msg = "\n";
                if (parameters.Length < 2)
                {
                    msg += "You must specify a name for the key to add.";
                }
                else if (parameters.Length >= 2)
                {
                    var namesAndGrants = parameters[1..];
                    //var indexOfComma = Array.IndexOf(namesAndGrants, namesAndGrants.FirstOrDefault(c => c.EndsWith(',')));
                    var indexOfParamWithComma = Array.IndexOf(namesAndGrants, namesAndGrants.FirstOrDefault(c => c.Contains(',')));

                    var name = "";
                    var grants = Array.Empty<string>();
                    if (indexOfParamWithComma > -1)
                    {
                        foreach (var item in namesAndGrants[..(indexOfParamWithComma + 1)])
                            name += $"{item.TrimEnd(',')} ";
                        name = name.TrimEnd();

                        //msg += $"nameBeforeSplit: {name}\n";

                        if (name.Contains(','))
                        {
                            var nameSplit = name.Split(',');

                            //msg += $"nameSplit: ";
                            //Array.ForEach(nameSplit, x => msg += $"{x} ");
                            //msg += $"\n";

                            name = nameSplit[0].TrimEnd();
                            grants = grants.AddRangeToArray(nameSplit[1..]);
                        }

                        grants = grants.AddRangeToArray(namesAndGrants[(indexOfParamWithComma + 1)..]);
                    }
                    else
                    {
                        foreach (var item in namesAndGrants)
                            name += $"{item.TrimEnd(',')} ";
                        name = name.TrimEnd();
                    }

                    //msg += $"namesAndGrants: ";
                    //Array.ForEach(namesAndGrants, x => msg += $"{x} ");
                    //msg += $"\n";
                    //msg += $"indexOfComma: {indexOfParamWithComma}\n";
                    //msg += $"Name: {name}\n";
                    //msg += $"Grants: ";
                    //Array.ForEach(grants, x => msg += $"{x} ");
                    //msg = msg.TrimEnd();
                    //msg += $"\n";

                    var success = APIKeys.Add(name, grants, out var apiKey);

                    if (success)
                    {
                        msg += "=========================================================\n";
                        msg += $"Name: {apiKey.Name}\n";
                        msg += $"Key: {apiKey.Key}\n";
                        msg += $"Grants: {string.Join("; ", apiKey.Grants)}\n";
                        msg += "=========================================================\n";
                    }
                    else
                    {
                        msg += "Unable to generate a key.";
                    }
                }
                WriteOutputInfo(session, msg, ChatMessageType.WorldBroadcast);
            }
            else if (parameters[0] == "modifykey")
            {
            }
            else if (parameters[0] == "revokekey")
            {
                var msg = "\n";
                if (parameters.Length < 2)
                {
                    msg += "You must specify a name for the key to revoke.";
                }
                else if (parameters.Length >= 2)
                {
                    var name = string.Join(" ", parameters[1..]);

                    var success = APIKeys.Remove(name);

                    if (success)
                    {
                        msg += "Key revoked.";
                    }
                    else
                    {
                        msg += "Unable to revoke that key.";
                    }
                }
                WriteOutputInfo(session, msg, ChatMessageType.WorldBroadcast);
            }
        }

        /// <summary>
        /// This will determine where a command handler should output to, the console or a client session.<para />
        /// If the session is null, the output will be sent to the console. If the session is not null, and the session.Player is in the world, it will be sent to the session.<para />
        /// Messages sent to the console will be sent using log.Info()
        /// </summary>
        public static void WriteOutputInfo(Session session, string output, ChatMessageType chatMessageType = ChatMessageType.Broadcast)
        {
            if (session != null)
            {
                if (session.State == Server.Network.Enum.SessionState.WorldConnected && session.Player != null)
                    ChatPacket.SendServerMessage(session, output, chatMessageType);
            }
            else
                Mod.Log(output, ModManager.LogLevel.Info);
        }

        /// <summary>
        /// This will determine where a command handler should output to, the console or a client session.<para />
        /// If the session is null, the output will be sent to the console. If the session is not null, and the session.Player is in the world, it will be sent to the session.<para />
        /// Messages sent to the console will be sent using log.Debug()
        /// </summary>
        public static void WriteOutputDebug(Session session, string output, ChatMessageType chatMessageType = ChatMessageType.Broadcast)
        {
            if (session != null)
            {
                if (session.State == Server.Network.Enum.SessionState.WorldConnected && session.Player != null)
                    ChatPacket.SendServerMessage(session, output, chatMessageType);
            }
            else
                Mod.Log(output, ModManager.LogLevel.Debug);
        }

        /// <summary>
        /// This will determine where a command handler should output to, the console or a client session.<para />
        /// If the session is null, the output will be sent to the console. If the session is not null, and the session.Player is in the world, it will be sent to the session.<para />
        /// Messages sent to the console will be sent using log.Debug()
        /// </summary>
        public static void WriteOutputError(Session session, string output, ChatMessageType chatMessageType = ChatMessageType.Broadcast)
        {
            if (session != null)
            {
                if (session.State == Server.Network.Enum.SessionState.WorldConnected && session.Player != null)
                    ChatPacket.SendServerMessage(session, output, chatMessageType);
            }
            else
                Mod.Log(output, ModManager.LogLevel.Error);
        }
    }
}
