using Terraria.ModLoader;

namespace AsymmetricEquips.Content.ModSupport.ClickerClass;

[JITWhenModsEnabled(ModName)]
public sealed class ClickerClassBasicSupport : ModSystem
{
	private const string ModName = "ClickerClass";

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(ModName);
	}

	public override void PostSetupContent()
	{
		ModSupportHelpers.AddHiddenEquip(ModName, "AimAssistModule", EquipType.Face);
		ModSupportHelpers.AddHiddenEquip(ModName, "IcePack", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "Milk", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "Soda", EquipType.Waist);

		// TODO: Need flipped sprites for:
		// - Gamer Crate
		// - All three Clicking Gloves (Normal, Ancient, Regal) [Need offhand]
	}
}