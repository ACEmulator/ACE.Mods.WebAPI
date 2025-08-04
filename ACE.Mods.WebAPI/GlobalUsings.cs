global using ACE.Common;
global using ACE.Database;
global using ACE.Database.Models.Shard;
global using ACE.Database.Models.World;
global using ACE.DatLoader.Entity.AnimationHooks;
global using ACE.Entity;
global using ACE.Entity.Enum;
global using ACE.Entity.Enum.Properties;
global using ACE.Entity.Models;

global using ACE.Server.Command;
global using ACE.Server.Command.Handlers;
global using ACE.Server.Entity;
global using ACE.Server.Entity.Actions;
global using ACE.Server.Factories;
global using ACE.Server.Factories.Enum;
global using ACE.Server.Factories.Entity;
global using ACE.Server.Factories.Tables;
global using ACE.Server.Managers;
global using ACE.Server.Mods;
global using ACE.Server.Network.GameEvent.Events;
global using ACE.Server.Network.GameMessages.Messages;
global using ACE.Server.Network;
global using ACE.Server.WorldObjects.Entity;
global using ACE.Server.WorldObjects;

global using HarmonyLib;

global using System.Diagnostics;
global using System.Numerics;
global using System.Reflection;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;

global using ACE.Shared;
global using ACE.Shared.Helpers;
global using ACE.Shared.Mods;

global using Biota = ACE.Entity.Models.Biota;
global using Weenie = ACE.Entity.Models.Weenie;
global using Spell = ACE.Server.Entity.Spell;

#if REALM
global using ACE.Server.Realms;
global using Session = ACE.Server.Network.ISession;
global using BinaryWriter = ACE.Server.Network.GameMessages.RealmsBinaryWriter;
global using Position = ACE.Server.Realms.InstancedPosition;
#endif

global using GenHTTP.Api.Content.Authentication;
global using GenHTTP.Api.Infrastructure;
global using GenHTTP.Api.Protocol;

global using GenHTTP.Modules.Authentication;
global using GenHTTP.Modules.Authentication.ApiKey;

global using GenHTTP.Engine.Internal;

global using GenHTTP.Modules.ApiBrowsing;
global using GenHTTP.Modules.Controllers;
global using GenHTTP.Modules.IO;
global using GenHTTP.Modules.Layouting;
global using GenHTTP.Modules.OpenApi;
global using GenHTTP.Modules.Practices;
global using GenHTTP.Modules.Webservices;

global using ACE.Mods.WebAPI.Controllers;
global using ACE.Mods.WebAPI.WebServices;
