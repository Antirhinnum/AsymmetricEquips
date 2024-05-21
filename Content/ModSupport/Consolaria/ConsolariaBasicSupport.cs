using Terraria.ModLoader;

namespace AsymmetricEquips.Content.ModSupport.Consolaria;

[JITWhenModsEnabled(ModName)]
public sealed class ConsolariaBasicSupport : ModSystem
{
	private const string ModName = "Consolaria";

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(ModName);
	}

	public override void PostSetupContent()
	{
		ModSupportHelpers.AddHiddenEquip(ModName, "ValentineRing", EquipType.HandsOn);

		// TODO: Flipped equips for the following:
		// - PhantasmalRobe
		// - AlpineHat
		// - FabulousRibbon (?)
		// - GeorgesTuxedo (Cane)
		// - MonokumaBody, MonokumaHead, MonokumaLegs
		// - MonomiBody, MonomiHead, MonomiLegs
		// - MythicalRobe
		// - ShirenShirt
	}
}