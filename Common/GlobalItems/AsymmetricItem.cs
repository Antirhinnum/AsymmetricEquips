using AsymmetricEquips.Common.Data;
using AsymmetricEquips.Common.Players;
using AsymmetricEquips.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AsymmetricEquips.Common.GlobalItems;

public sealed class AsymmetricItem : GlobalItem
{
	/// <summary>
	/// The side of the player this equip is on.
	/// </summary>
	public PlayerSide Side { get; internal set; }

	/// <summary>
	/// The <see cref="EquipType.Balloon"/> equip slot this item uses.<br/>
	/// This is what <see cref="AsymmetricPlayer.frontBalloon"/> and <see cref="AsymmetricPlayer.frontBalloonInner"/> are set to.
	/// </summary>
	public sbyte frontBalloonSlot = -1;

	// Items should not share equip slots or player sides.
	public override bool InstancePerEntity => true;

	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return lateInstantiation && AsymmetricSystem.ItemIsAsymmetrical(entity);
	}

	/// <summary>
	/// Overwrites an item's equip slots to match up with the side it's supposed to draw on.
	/// </summary>
	/// <param name="modifyPlayer">If true, change ModPlayer / Player fields.</param>
	/// <returns>A cache of the original item's equip slots.</returns>
	internal static EquipData HandleAsymmetricism(Item equip, Player player)
	{
		EquipData cache = new(equip ?? new Item());
		if (equip == null || equip.IsAir || player == null)
		{
			return cache;
		}

		if (!equip.TryGetGlobalItem(out AsymmetricItem aItem))
		{
			return cache;
		}

		PlayerSide side = aItem.Side;

		if (side == PlayerSide.Default)
		{
			return cache;
		}

		bool leftSideEquip = side == PlayerSide.Left;
		bool facingLeft = player.direction == -1;
		IReadOnlyDictionary<(EquipType, int), AsymmetricData> asymmetricsByEquip = AsymmetricSystem.AsymmetricsByEquip;

		AsymmetricData asymmetricData;
		if (leftSideEquip != facingLeft)
		{
			// Only equips that default to the player's front side are updated here
			if (equip.headSlot > 0 && asymmetricsByEquip.TryGetValue((EquipType.Head, equip.headSlot), out asymmetricData))
			{
				equip.headSlot = asymmetricData.newId;
				if (asymmetricData.newId == ArmorIDs.Head.FamiliarWig && player.TryGetModPlayer(out AsymmetricPlayer aPlayer))
				{
					aPlayer.flippedToHair = true;
				}
			}
			if (equip.bodySlot > 0 && asymmetricsByEquip.TryGetValue((EquipType.Body, equip.bodySlot), out asymmetricData))
			{
				equip.bodySlot = asymmetricData.newId;
			}
			if (equip.legSlot > 0 && asymmetricsByEquip.TryGetValue((EquipType.Legs, equip.legSlot), out asymmetricData))
			{
				equip.legSlot = asymmetricData.newId;
			}
			if (equip.handOnSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.HandsOn, equip.handOnSlot), out asymmetricData))
			{
				equip.handOnSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.backSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Back, equip.backSlot), out asymmetricData))
			{
				equip.backSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.frontSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Front, equip.frontSlot), out asymmetricData))
			{
				equip.frontSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.shoeSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Shoes, equip.shoeSlot), out asymmetricData))
			{
				equip.shoeSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.waistSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Waist, equip.waistSlot), out asymmetricData))
			{
				equip.waistSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.wingSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Wings, equip.wingSlot), out asymmetricData))
			{
				equip.wingSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.shieldSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Shield, equip.shieldSlot), out asymmetricData))
			{
				equip.shieldSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.neckSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Neck, equip.neckSlot), out asymmetricData))
			{
				equip.neckSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.faceSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Face, equip.faceSlot), out asymmetricData))
			{
				equip.faceSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.beardSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Beard, equip.beardSlot), out asymmetricData))
			{
				equip.beardSlot = (sbyte)asymmetricData.newId;
			}
		}
		else
		{
			// Only equips that default to the player's back side are changed here
			if (equip.handOffSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.HandsOff, equip.handOffSlot), out asymmetricData))
			{
				equip.handOffSlot = (sbyte)asymmetricData.newId;
			}
			if (equip.balloonSlot > 0 && asymmetricsByEquip.TryGetValue(((EquipType, int))(EquipType.Balloon, equip.balloonSlot), out asymmetricData))
			{
				equip.GetGlobalItem<AsymmetricItem>().frontBalloonSlot = equip.balloonSlot;
				equip.balloonSlot = (sbyte)asymmetricData.newId;
			}
		}

		return cache;
	}

	/// <summary>
	/// Adds an extra tooltip to items that are flippable.
	/// </summary>
	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		// Only tell players they can flip items if they're in a slot that's syncable.
		if (AsymmetricSystem._allowedFlipContexts.Contains(item.tooltipContext))
		{
			TooltipLine line = new(Mod, $"{nameof(AsymmetricEquips)}:{nameof(Side)}", Language.GetTextValue($"Mods.{nameof(AsymmetricEquips)}.ExtraTooltip.FlipPossible"))
			{
				OverrideColor = Color.Salmon
			};
			tooltips.Add(line);
		}

		// Only tell players an item's side if it's not default.
		if (Side != PlayerSide.Default)
		{
			TooltipLine line = new(Mod, $"{nameof(AsymmetricEquips)}:{nameof(Side)}", Language.GetTextValue($"Mods.{nameof(AsymmetricEquips)}.ExtraTooltip.{Side}"))
			{
				OverrideColor = Color.Salmon
			};
			tooltips.Add(line);
		}
	}

	#region Save

	public override void SaveData(Item item, TagCompound tag)
	{
		if (Side != PlayerSide.Default)
		{
			tag.Add(nameof(Side), (byte)Side);
		}
	}

	public override void LoadData(Item item, TagCompound tag)
	{
		if (tag.TryGet(nameof(Side), out byte side))
		{
			Side = (PlayerSide)side;
		}
	}

	#endregion Save

	#region Sync

	public override void NetSend(Item item, BinaryWriter writer)
	{
		writer.Write((byte)Side);
	}

	public override void NetReceive(Item item, BinaryReader reader)
	{
		Side = (PlayerSide)reader.ReadByte();
	}

	#endregion Sync
}