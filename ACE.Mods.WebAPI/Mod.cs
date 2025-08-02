namespace ACE.Mods.WebAPI;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(WebAPI), new PatchClass(this));
}
