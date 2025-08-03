namespace ACE.Mods.WebAPI;

public class Mod : BasicMod
{
    public Mod() : base() => Setup("ACE.Mods.WebAPI", new PatchClass(this));

    internal static void Log(string message, ModManager.LogLevel level = ModManager.LogLevel.Info)
    {
        ModManager.Log($"[ACE.Mods.WebAPI] {message}", level);
    }
}
