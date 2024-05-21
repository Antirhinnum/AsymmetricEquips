using Terraria.ModLoader;

namespace AsymmetricEquips.Content.ModSupport.TheDepths;

[JITWhenModsEnabled(ModName)]
public sealed class TheDepthsBasicSupport : ModSystem
{
	private const string ModName = "TheDepths";

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(ModName);
	}

	public override void PostSetupContent()
	{
		ModSupportHelpers.AddHiddenEquip(ModName, "StoneRose", EquipType.Face);
		ModSupportHelpers.AddGlove(ModName, "AquaGlove");
	}
}