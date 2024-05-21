using Terraria;
using Terraria.ModLoader;

namespace AsymmetricEquips.Content.ModSupport.Liber;

[JITWhenModsEnabled(ModName)]
public sealed class LiberBasicSupport : ModSystem
{
	private const string ModName = "Liber";

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(ModName);
	}

	public override void OnModLoad()
	{
		if (!Main.dedServ)
		{
			ModSupportHelpers.AddFlippedEquipTexture(ModName, "VikingShield", EquipType.Shield);
		}
	}

	public override void PostSetupContent()
	{
		ModSupportHelpers.AddHiddenEquip(ModName, "SkullRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "WristBand", EquipType.HandsOn);
		ModSupportHelpers.AddGlove(ModName, "Gauntlet");
		ModSupportHelpers.AddFlippedEquip(ModName, "VikingShield", EquipType.Shield);
	}
}