using Terraria.ModLoader;

namespace AsymmetricEquips.Content.ModSupport.DBZMODPORT;

[JITWhenModsEnabled(ModName)]
public sealed class DBZMODPORTBasicSupport : ModSystem
{
	private const string ModName = "DBZMODPORT";

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(ModName);
	}

	public override void PostSetupContent()
	{
		ModSupportHelpers.AddHiddenEquip(ModName, "GreenPotara", EquipType.Face);
		ModSupportHelpers.AddHiddenEquip(ModName, "BattleKit", EquipType.Face);
		ModSupportHelpers.AddGlove(ModName, "BattleKit");
		ModSupportHelpers.AddGlove(ModName, "WornGloves");

		static void AddScouter(string itemName)
		{
			ModSupportHelpers.AddSmallHead(ModName, itemName);
			ModSupportHelpers.AddHiddenEquip(ModName, itemName, EquipType.Face);
		}

		AddScouter("ScouterT2");
		AddScouter("ScouterT3");
		AddScouter("ScouterT4");
		AddScouter("ScouterT5");
		AddScouter("ScouterT6");
	}
}