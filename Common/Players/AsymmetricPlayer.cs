using AsymmetricEquips.Common.Data;
using AsymmetricEquips.Common.GlobalItems;
using AsymmetricEquips.Common.Systems;
using MonoMod.Utils;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.UI;

namespace AsymmetricEquips.Common.Players;

/// <summary>
/// Stores custom equip and dye slots. Also handles player hooks.
/// </summary>
public sealed class AsymmetricPlayer : ModPlayer
{
	// Required to sync equips in modded accessory slots.
	private static MethodInfo _ModAccessorySlotPlayer_NetHandler_SendSlot;

	/// <summary>
	/// The <see cref="EquipType.Balloon"/> equip slot that is drawn over the player.<br/>
	/// Handles balloons where <see cref="ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer"/> is <see langword="false"/>.
	/// </summary>
	public int frontBalloon;

	/// <summary>
	/// The <see cref="EquipType.Balloon"/> equip slot that is drawn over the player, but under their arm.<br/>
	/// Handles balloons where <see cref="ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer"/> is <see langword="true"/>.
	/// </summary>
	public int frontBalloonInner;

	/// <summary>
	/// The index of the armor shader applied to <see cref="frontBalloon"/>.
	/// </summary>
	public int cFrontBalloon;

	/// <summary>
	/// The index of the armor shader applied to <see cref="frontBalloonInner"/>.
	/// </summary>
	public int cFrontBalloonInner;

	/// <summary>
	/// If true, then one of the player's equips has changed their head equip to <see cref="ArmorIDs.Head.FamiliarWig"/>.<br/>
	/// Used to prevent hair drawing if it normally would be prevented, but the logic didn't work because of asymmetricism.<br/>
	/// </summary>
	/// <remarks>
	/// Without this, wearing an Eye Patch and an Obsidian Skull would show hair if the eyepatch wasn't visible.
	/// </remarks>
	public bool flippedToHair;

	private static readonly FastReflectionHelper.FastInvoker _playerUpdateItemDye = typeof(Player).GetMethod("UpdateItemDye", BindingFlags.Instance | BindingFlags.NonPublic).GetFastInvoker();

	public override void Load()
	{
		_ModAccessorySlotPlayer_NetHandler_SendSlot = typeof(ModAccessorySlotPlayer).GetNestedType("NetHandler", BindingFlags.NonPublic | BindingFlags.Static).GetMethod("SendSlot", BindingFlags.Public | BindingFlags.Static);

		On_Player.GetHairSettings += HideHairForAsymmetrics;
		On_Player.PlayerFrame += HijackFrame;
		On_Player.ResetVisibleAccessories += ResetCustomEquipSlots;
		On_Player.UpdateDyes += ResetCustomDyeSlots;
		On_Player.UpdateItemDye += UpdateDyeSlots;
		On_Player.UpdateVisibleAccessory += UpdateEquipSlots;
	}

	public override void Unload()
	{
		_ModAccessorySlotPlayer_NetHandler_SendSlot = null;

		On_Player.GetHairSettings -= HideHairForAsymmetrics;
		On_Player.PlayerFrame -= HijackFrame;
		On_Player.ResetVisibleAccessories -= ResetCustomEquipSlots;
		On_Player.UpdateDyes -= ResetCustomDyeSlots;
		On_Player.UpdateItemDye -= UpdateDyeSlots;
		On_Player.UpdateVisibleAccessory -= UpdateEquipSlots;
	}

	/// <summary>
	/// Most head equip asymmetrics set newId to ArmorIDs.Head.FamiliarHair (0) since it looks more natural for the equip to vanish than to be replaced by the player's helmet.<br/>
	/// However, doing this fucks with hair drawing: If faceHead >= 0, then hair is only allowed to draw if head == 0.<br/>
	/// This makes hair appear/disappear when the player turns around.<br/>
	/// This hook fixes that. It also makes sure that dyes don't transfer over to the head.
	/// </summary>
	private void HideHairForAsymmetrics(On_Player.orig_GetHairSettings orig, Player self, out bool fullHair, out bool hatHair, out bool hideHair, out bool backHairDraw, out bool drawsBackHairWithoutHeadgear)
	{
		orig(self, out fullHair, out hatHair, out hideHair, out backHairDraw, out drawsBackHairWithoutHeadgear);

		if (self.TryGetModPlayer(out AsymmetricPlayer aPlayer) && aPlayer.flippedToHair)
		{
			hideHair |= self.faceHead >= 0;
			self.cHead = 0;
		}
	}

	/// <summary>
	/// Handles actually making equips show or not show based on their side.
	/// </summary>
	private static void HijackFrame(On_Player.orig_PlayerFrame orig, Player self)
	{
		#region Setup

		int vanillaLength = self.armor.Length;
		EquipData?[] data = new EquipData?[vanillaLength];
		for (int i = 0; i < vanillaLength; i++)
		{
			if (self.IsItemSlotUnlockedAndUsable(i))
			{
				data[i] = AsymmetricItem.HandleAsymmetricism(self.armor[i], self);
			}
		}

		EquipData?[] moddedData = null;
		if (self.TryGetModPlayer(out ModAccessorySlotPlayer mPlayer))
		{
			int slots = mPlayer.SlotCount;
			moddedData = new EquipData?[slots * 2];
			AccessorySlotLoader loader = LoaderManager.Get<AccessorySlotLoader>();
			for (int i = 0; i < slots; i++)
			{
				if (loader.ModdedIsItemSlotUnlockedAndUsable(i, self))
				{
					ModAccessorySlot modAccessorySlot = loader.Get(i, self);
					moddedData[i] = AsymmetricItem.HandleAsymmetricism(modAccessorySlot.FunctionalItem, self);
					moddedData[i + slots] = AsymmetricItem.HandleAsymmetricism(modAccessorySlot.VanityItem, self);
				}
			}
		}

		#endregion Setup

		orig(self);

		// If no items are asymmetric, then there's nothing to clean up.
		if (data.All(a => a == null) && (moddedData == null || moddedData!.All(a => a == null)))
		{
			return;
		}

		// UpdateDyes usually runs way earlier than PlayerFrame, which results in a one-frame delay between an item switching sides and its dye applying.
		// This fixes that by updating dyes using the new equip slots.

		#region Custom Dye Logic

		// Some equips that change slots to be asymmetric (ex: handsOn/handsOff) have a one-frame delay before the correct dye is applied.
		// This used to be fixed by calling Player.UpdateDyes() again, but that caused other issues (specifically, the Tax Collector's Suit wasn't getting dyed correctly).
		// Now, dyes are manually reassigned for these equips.

		for (int i = 0; i < 20; i++)
		{
			if (self.IsItemSlotUnlockedAndUsable(i))
			{
				int armorSlot = i % 10;
				_playerUpdateItemDye.Invoke(self, new object[] { i < 10, self.hideVisibleAccessory[armorSlot], self.armor[i], self.dye[armorSlot] });
			}
		}

		if (mPlayer != null)
		{
			AccessorySlotLoader loader = LoaderManager.Get<AccessorySlotLoader>();
			for (int i = 0; i < mPlayer.SlotCount; i++)
			{
				if (loader.ModdedIsItemSlotUnlockedAndUsable(i, self))
				{
					ModAccessorySlot modAccessorySlot = loader.Get(i, self);
					_playerUpdateItemDye.Invoke(self, new object[] { true, modAccessorySlot.HideVisuals, modAccessorySlot.FunctionalItem, modAccessorySlot.DyeItem });
					_playerUpdateItemDye.Invoke(self, new object[] { false, modAccessorySlot.HideVisuals, modAccessorySlot.VanityItem, modAccessorySlot.DyeItem });
				}
			}
		}

		#endregion Custom Dye Logic

		#region Restore

		for (int i = 0; i < vanillaLength; i++)
		{
			if (data[i] != null && self.IsItemSlotUnlockedAndUsable(i))
			{
				data[i].Value.Retrieve(self.armor[i]);
			}
		}

		if (mPlayer != null)
		{
			int slots = mPlayer.SlotCount;
			AccessorySlotLoader loader = LoaderManager.Get<AccessorySlotLoader>();
			for (int i = 0; i < slots; i++)
			{
				if (loader.ModdedIsItemSlotUnlockedAndUsable(i, self))
				{
					ModAccessorySlot modAccessorySlot = loader.Get(i, self);
					moddedData[i].Value.Retrieve(modAccessorySlot.FunctionalItem);
					moddedData[i + slots].Value.Retrieve(modAccessorySlot.VanityItem);
				}
			}
		}

		#endregion Restore
	}

	/// <summary>
	/// Updates dyes for equips that swap slots to be asymmetric.<br/>
	/// Specifically, <see cref="EquipType.HandsOn"/> and <see cref="EquipType.HandsOff"/>.
	/// </summary>
	private static void UpdateSwappedDyes(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
	{
		if (armorItem.IsAir || (isNotInVanitySlot && isSetToHidden))
			return;

		if (armorItem.handOnSlot > 0)
			player.cHandOn = dyeItem.dye;

		if (armorItem.handOffSlot > 0)
			player.cHandOff = dyeItem.dye;

		if (armorItem.balloonSlot > 0)
		{
			if (ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer[armorItem.balloonSlot])
				player.cBalloonFront = dyeItem.dye;
			else
				player.cBalloon = dyeItem.dye;
		}

		if (armorItem.TryGetGlobalItem(out AsymmetricItem aItem) && aItem.frontBalloonSlot > -1 && player.TryGetModPlayer(out AsymmetricPlayer aPlayer))
		{
			if (ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer[aItem.frontBalloonSlot])
				aPlayer.cFrontBalloonInner = dyeItem.dye;
			else
				aPlayer.cFrontBalloon = dyeItem.dye;
		}
	}

	/// <summary>
	/// Resets custom equip slots back to -1.
	/// </summary>
	private static void ResetCustomEquipSlots(On_Player.orig_ResetVisibleAccessories orig, Player self)
	{
		if (self.TryGetModPlayer(out AsymmetricPlayer aPlayer))
		{
			aPlayer.frontBalloon = -1;
			aPlayer.frontBalloonInner = -1;
			aPlayer.flippedToHair = false;
		}

		orig(self);
	}

	/// <summary>
	/// Resets custom dye slots back to -1.
	/// </summary>
	private static void ResetCustomDyeSlots(On_Player.orig_UpdateDyes orig, Player self)
	{
		if (self.TryGetModPlayer(out AsymmetricPlayer aPlayer))
		{
			aPlayer.cFrontBalloon = 0;
			aPlayer.cFrontBalloonInner = 0;
		}

		orig(self);
	}

	/// <summary>
	/// Updates custom dye slots and ensures that dyes are applied to the correct equip.<br/>
	/// For example, this ensures that the player can wear and dye two gloves independently.
	/// </summary>
	private static void UpdateDyeSlots(On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
	{
		// Don't run any extra code for non-asymmetric items.
		if (!armorItem.TryGetGlobalItem(out AsymmetricItem aItem))
		{
			orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
			return;
		}

		EquipData data = AsymmetricItem.HandleAsymmetricism(armorItem, self);
		orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);

		// Handle custom dyes
		if (aItem.frontBalloonSlot > -1 && self.TryGetModPlayer(out AsymmetricPlayer aPlayer))
		{
			if (ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer[aItem.frontBalloonSlot])
				aPlayer.cFrontBalloonInner = dyeItem.dye;
			else
				aPlayer.cFrontBalloon = dyeItem.dye;
		}

		data.Retrieve(armorItem);
	}

	/// <summary>
	/// Updates custom equip slots and ensures that the player's equips are set correctly.<br/>
	/// For example, this ensures that the player can wear two gloves independently.
	/// </summary>
	private static void UpdateEquipSlots(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
	{
		// Don't run any extra code for non-asymmetric items.
		if (!item.TryGetGlobalItem(out AsymmetricItem aItem))
		{
			orig(self, itemSlot, item, modded);
			return;
		}

		EquipData data = AsymmetricItem.HandleAsymmetricism(item, self);
		orig(self, itemSlot, item, modded);

		if (aItem.frontBalloonSlot > -1 && self.TryGetModPlayer(out AsymmetricPlayer aPlayer))
		{
			if (ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer[aItem.frontBalloonSlot])
			{
				aPlayer.frontBalloonInner = aItem.frontBalloonSlot;
			}
			else
			{
				aPlayer.frontBalloon = aItem.frontBalloonSlot;
			}
		}

		data.Retrieve(item);
	}

	public override void FrameEffects()
	{
		// Call FrameEffects on frontBalloons.
		EquipTexture frontBalloonEquip = EquipLoader.GetEquipTexture(EquipType.Balloon, frontBalloon);

		frontBalloonEquip?.FrameEffects(Player, EquipType.Balloon);
	}

	/// <summary>
	/// When the player Shift + Control + Clicks an item, try to
	/// </summary>
	public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
	{
		if (!AsymmetricSystem._allowedFlipContexts.Contains(context))
		{
			return base.ShiftClickSlot(inventory, context, slot);
		}

		if (ItemSlot.ControlInUse && inventory[slot].TryGetGlobalItem(out AsymmetricItem aItem))
		{
			SoundEngine.PlaySound(SoundID.Grab);
			aItem.Side = aItem.Side.NextEnum();

			// Sync this change
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (context == ItemSlot.Context.InventoryItem) // Player inventory
				{
					NetMessage.SendData(MessageID.SyncEquipment, -1, Main.myPlayer, null, Main.myPlayer, slot);
				}
				else if (context < 0) // Modded equip slots
				{
					_ModAccessorySlotPlayer_NetHandler_SendSlot.Invoke(null, new object[] { -1, Main.myPlayer, slot, inventory[slot] });
				}
				else // Vanilla equip slots
				{
					NetMessage.SendData(MessageID.SyncEquipment, -1, Main.myPlayer, null, Main.myPlayer, PlayerItemSlotID.Armor0 + slot);
				}

				// This should make Mannequins work, but Shift-Control-Clicking items still moves them. Oh well.
				//else if ((context == ItemSlot.Context.DisplayDollArmor || context == ItemSlot.Context.DisplayDollAccessory) && Main.LocalPlayer.tileEntityAnchor.IsInValidUseTileEntity() && Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEDisplayDoll displayDoll)
				//{
				//	NetMessage.SendData(MessageID.TEDisplayDollItemSync, -1, -1, null, Main.myPlayer, displayDoll.ID, slot);
				//}
			}

			// Don't move the item to any other inventory.
			return true;
		}

		return base.ShiftClickSlot(inventory, context, slot);
	}
}