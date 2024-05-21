using Terraria.ModLoader;

namespace AsymmetricEquips.Content.ModSupport.CalamityMod;

[JITWhenModsEnabled(ModName)]
public sealed class CalamityModBasicSupport : ModSystem
{
	private const string ModName = "CalamityMod";

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(ModName);
	}

	public override void PostSetupContent()
	{
		ModSupportHelpers.AddHiddenEquip(ModName, "DepthCharm", EquipType.Waist);
		ModSupportHelpers.AddGlove(ModName, "BloodstainedGlove");
		ModSupportHelpers.AddGlove(ModName, "ElectriciansGlove");
		ModSupportHelpers.AddGlove(ModName, "ElementalGauntlet");
		ModSupportHelpers.AddGlove(ModName, "FilthyGlove");
		ModSupportHelpers.AddGlove(ModName, "GloveOfPrecision");
		ModSupportHelpers.AddGlove(ModName, "GloveOfRecklessness");
	}
}