using Terraria.ModLoader;

namespace AsymmetricEquips.Content.ModSupport.BalloonsExtended;

[JITWhenModsEnabled(ModName)]
public sealed class BalloonsExtendedBasicSupport : ModSystem
{
	private const string ModName = "BalloonsExtended";

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(ModName);
	}

	public override void PostSetupContent()
	{
		// Every single accessory in this mod has an asymmetric Balloon equip.
		foreach (ModItem item in ModLoader.GetMod(ModName).GetContent<ModItem>())
		{
			if (item.Item.balloonSlot > -1)
			{
				ModSupportHelpers.AddBalloon(item.Type);
			}
		}
	}
}