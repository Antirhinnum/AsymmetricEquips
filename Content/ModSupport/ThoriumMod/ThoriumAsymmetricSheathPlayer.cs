using AsymmetricEquips.Common.GlobalItems;
using AsymmetricEquips.Common.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.PlayerLayers;

namespace AsymmetricEquips.Content.ModSupport.ThoriumMod;

[JITWhenModsEnabled(BaseMod)]
internal sealed class ThoriumAsymmetricSheathPlayer : ModPlayer
{
	private const string BaseMod = "ThoriumMod";

	/// <summary>
	/// The current <see cref="AccessoryType.SwordSheath"/> that this player has equipped.
	/// </summary>
	internal Item currentSheath;

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(BaseMod);
	}

	public override void Load()
	{
		On_Player.UpdateVisibleAccessory += TrackVisibleSheath;
	}

	public override void Unload()
	{
		On_Player.UpdateVisibleAccessory -= TrackVisibleSheath;
	}

	private static void TrackVisibleSheath(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
	{
		orig(self, itemSlot, item, modded);

		if (item.ModItem is ThoriumItem tItem && tItem.accessoryType == AccessoryType.SwordSheath && self.TryGetModPlayer(out ThoriumAsymmetricSheathPlayer aPlayer))
		{
			aPlayer.currentSheath = item;
		}
	}

	public override void SetStaticDefaults()
	{
		foreach (ThoriumItem tItem in ModContent.GetContent<ThoriumItem>())
		{
			if (tItem.accessoryType == AccessoryType.SwordSheath)
			{
				AsymmetricSystem.AddSpecialItem(tItem.Type, PlayerSide.Left);
			}
		}
	}

	public override void ResetEffects()
	{
		currentSheath = null;
	}

	public override void ModifyDrawLayerOrdering(IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions)
	{
		// Default position is before BackAcc.
		positions[ModContent.GetInstance<SheathLayer>()] = new PlayerDrawLayer.Multiple()
		{
			{ new PlayerDrawLayer.Between(PlayerDrawLayers.HairBack, PlayerDrawLayers.BackAcc), drawInfo => ShouldDrawSheathBack(drawInfo.drawPlayer) },
			{ new PlayerDrawLayer.Between(PlayerDrawLayers.WaistAcc, PlayerDrawLayers.NeckAcc), drawInfo => !ShouldDrawSheathBack(drawInfo.drawPlayer) }
		};
	}

	private static bool ShouldDrawSheathBack(Player player)
	{
		// Always draw the back sheath if we can't check the sheath for asymmetricism.
		if (!player.TryGetModPlayer(out ThoriumAsymmetricSheathPlayer sheathPlayer))
		{
			return true;
		}

		if (sheathPlayer.currentSheath is not Item sheath)
		{
			return true;
		}

		return sheath.TryGetGlobalItem(out AsymmetricItem asymmetricItem) && asymmetricItem.ItemOnDefaultSide(sheath, player);
	}
}