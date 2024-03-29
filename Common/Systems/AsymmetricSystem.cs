﻿using AsymmetricEquips.Common.Data;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AsymmetricEquips.Common.Systems;

public enum PlayerSide
{
	Default,
	Left,
	Right
}

/// <summary>
/// Handles all of the static data for this mod.
/// </summary>
public sealed class AsymmetricSystem : ModSystem
{
	/// <summary>
	/// The ItemSlot contexts that the player can flip an equip's side in.<br/>
	/// </summary>
	/// <remarks>
	/// This is required for two main reasons:<br/>
	/// 1. So the player can't flip items in, say, NPC shops.<br/>
	/// 2. So that the flip can be properly synced in MP.<br/>
	/// </remarks>
	internal static readonly int[] _allowedFlipContexts = new int[]
	{
		ItemSlot.Context.InventoryItem,
		ItemSlot.Context.EquipArmor,
		ItemSlot.Context.EquipArmorVanity,
		ItemSlot.Context.EquipAccessory,
		ItemSlot.Context.EquipAccessoryVanity,
		ItemSlot.Context.ModdedAccessorySlot,
		ItemSlot.Context.ModdedVanityAccessorySlot
		//ItemSlot.Context.DisplayDollArmor,
		//ItemSlot.Context.DisplayDollAccessory
	};

	/// <summary>
	/// A list of equips to handle asymmetrically and how to handle them
	/// </summary>
	private static readonly List<AsymmetricData> _asymmetrics = new();

	/// <summary>
	/// Relates an <see cref="EquipType"/> and equip ID with an <see cref="AsymmetricData"/>.
	/// </summary>
	public static IReadOnlyDictionary<EquipSlot, AsymmetricData> AsymmetricsByEquip { get; private set; }

	/// <summary>
	/// A list of items to treat asymmetrically even though they aren't registered in <see cref="_asymmetrics"/>.<br/>
	/// Intended for items that have non-equip slot visuals.
	/// </summary>
	internal static readonly List<(int itemId, PlayerSide side)> _specialItems = new();

	/// <summary>
	/// If true, then no more equips can be added.
	/// </summary>
	internal static bool _finishedEquips;

	public override void Unload()
	{
		_asymmetrics.Clear();
		_specialItems.Clear();
		AsymmetricsByEquip = null;
	}

	// Asymmetrics can only be added before AddRecipes is called.
	public override void AddRecipes()
	{
		_finishedEquips = true;
		AsymmetricsByEquip = _asymmetrics.ToDictionary(a => new EquipSlot(a.equipType, a.id));
	}

	public override void SetStaticDefaults()
	{
		// TODO: Make these equips, then add them.
		//HeadAsymetricMatch = ArmorIDs.Head.Sets.Factory.CreateIntSet(
		//	ArmorIDs.Head.GraduationCapBlack, /**/,
		//	ArmorIDs.Head.GraduationCapBlue, /**/,
		//	ArmorIDs.Head.GraduationCapMaroon, /**/,
		//	ArmorIDs.Head.TwinMask,  /**/
		//);

		_asymmetrics.Clear();
		_asymmetrics.AddRange(new List<AsymmetricData>()
		{
			// Head equips
			//new(EquipType.Head, ArmorIDs.Head.JungleRose, newId: ArmorIDs.Head.FamiliarWig), // Turned into a Face equip.
			new(EquipType.Head, ArmorIDs.Head.EyePatch, newId: ArmorIDs.Head.FamiliarWig),
			new(EquipType.Head, ArmorIDs.Head.HeartHairpin, newId: ArmorIDs.Head.FamiliarWig),
			new(EquipType.Head, ArmorIDs.Head.StarHairpin, newId: ArmorIDs.Head.FamiliarWig),
			new(EquipType.Head, ArmorIDs.Head.SeashellHairpin, newId: ArmorIDs.Head.FamiliarWig),
			new(EquipType.Head, ArmorIDs.Head.GhostarSkullPin, newId: ArmorIDs.Head.FamiliarWig),
			new(EquipType.Head, ArmorIDs.Head.PrettyPinkRibbon, newId: ArmorIDs.Head.FamiliarWig),

			// Waist equips
			new(EquipType.Waist, ArmorIDs.Waist.BlizzardinaBottle),
			new(EquipType.Waist, ArmorIDs.Waist.CloudinaBottle),
			new(EquipType.Waist, ArmorIDs.Waist.FartinaJar),
			new(EquipType.Waist, ArmorIDs.Waist.SandstorminaBottle),
			new(EquipType.Waist, ArmorIDs.Waist.TsunamiinaBottle),
			new(EquipType.Waist, ArmorIDs.Waist.CopperWatch),
			new(EquipType.Waist, ArmorIDs.Waist.TinWatch),
			new(EquipType.Waist, ArmorIDs.Waist.SilverWatch),
			new(EquipType.Waist, ArmorIDs.Waist.TungstenWatch),
			new(EquipType.Waist, ArmorIDs.Waist.GoldWatch),
			new(EquipType.Waist, ArmorIDs.Waist.PlatinumWatch),
			new(EquipType.Waist, ArmorIDs.Waist.ManaFlower),
			new(EquipType.Waist, ArmorIDs.Waist.TreasureMagnet),

			// Face equips
			// Some of these could do with being mirrored, maybe?
			// Like, the Obsidian Rose doesn't vanish: It just moves behind the player's head
			new(EquipType.Face, ArmorIDs.Face.ArcaneFlower),
			new(EquipType.Face, ArmorIDs.Face.JungleRose),
			new(EquipType.Face, ArmorIDs.Face.NaturesGift),
			new(EquipType.Face, ArmorIDs.Face.ObsidianRose),
			new(EquipType.Face, ArmorIDs.Face.MoltenSkullRose, ArmorIDs.Face.LavaSkull),
			new(EquipType.Face, ArmorIDs.Face.MoltenSkullRoseAlt, ArmorIDs.Face.LavaSkullAlt),
			new(EquipType.Face, ArmorIDs.Face.ObsidianSkullRose, ArmorIDs.Face.ObsidianSkull),
			new(EquipType.Face, ArmorIDs.Face.ObsidianSkullRoseAlt, ArmorIDs.Face.ObsidianSkullAlt),

			// HandOn equips
			// Could be moved to the other hand / arm, but that would require sprites
			new(EquipType.HandsOn, ArmorIDs.HandOn.BandofRegeneration),
			new(EquipType.HandsOn, ArmorIDs.HandOn.BandofStarpower),
			new(EquipType.HandsOn, ArmorIDs.HandOn.CharmofMyths),
			new(EquipType.HandsOn, ArmorIDs.HandOn.DiamondRing),
			new(EquipType.HandsOn, ArmorIDs.HandOn.HuntressBuckler),
			new(EquipType.HandsOn, ArmorIDs.HandOn.LavaCharm),
			new(EquipType.HandsOn, ArmorIDs.HandOn.ManaRegenerationBand),
			new(EquipType.HandsOn, ArmorIDs.HandOn.MoonStone),
			new(EquipType.HandsOn, ArmorIDs.HandOn.SunStone),

			// Gloves
			new(EquipType.HandsOn, ArmorIDs.HandOn.FeralClaws),
			new(EquipType.HandsOff, ArmorIDs.HandOff.FeralClaws, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.FireGauntlet),
			new(EquipType.HandsOff, ArmorIDs.HandOff.FireGauntlet, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.HandWarmer),
			new(EquipType.HandsOff, ArmorIDs.HandOff.HandWarmer, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.MagicCuffs),
			new(EquipType.HandsOff, ArmorIDs.HandOff.MagicCuffs, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.MechanicalGlove),
			new(EquipType.HandsOff, ArmorIDs.HandOff.MechanicalGlove, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.PowerGlove),
			new(EquipType.HandsOff, ArmorIDs.HandOff.PowerGlove, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.ClimbingClaws),
			new(EquipType.HandsOff, ArmorIDs.HandOff.ClimbingClaws, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.Shackle),
			new(EquipType.HandsOff, ArmorIDs.HandOff.Shackle, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.TitanGlove),
			new(EquipType.HandsOff, ArmorIDs.HandOff.TitanGlove, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.CelestialCuffs),
			new(EquipType.HandsOff, ArmorIDs.HandOff.CelestialCuffs, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.YoyoGlove),
			new(EquipType.HandsOff, ArmorIDs.HandOff.YoyoGlove, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.BersekerGlove),
			new(EquipType.HandsOff, ArmorIDs.HandOff.BersekerGlove, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.FrogWebbing),
			new(EquipType.HandsOff, ArmorIDs.HandOff.FrogWebbing, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.BoneGlove),
			new(EquipType.HandsOff, ArmorIDs.HandOff.BoneGlove, side: PlayerSide.Left),
			new(EquipType.HandsOn, ArmorIDs.HandOn.HandOfCreation),
			new(EquipType.HandsOff, ArmorIDs.HandOff.HandOfCreation, side: PlayerSide.Left),

			// Balloon equips
			new(EquipType.Balloon, ArmorIDs.Balloon.AmberHorseshoeBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.BalloonPufferfish, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.BlizzardinaBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.BlueHorseshoeBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.BundleofBalloons, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.CloudinaBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.FartinaBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.GreenHorseshoeBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.HoneyBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.PinkHorseshoeBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.SandstorminaBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.SharkronBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.ShinyRedBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.WhiteHorseshoeBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.YellowHorseshoeBalloon, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.BalloonAnimal, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.BundledPartyBalloons, side: PlayerSide.Left),
			new(EquipType.Balloon, ArmorIDs.Balloon.HorseshoeBundle, side: PlayerSide.Left),

			// The Royal Scepter isn't here because there's no way to make it draw both under the player's hand and above their arm.
			// It also has a chunk of out it where the player usually holds it, which is painfully visible when flipped.
		});

		AddSpecialItem(ItemID.Yoraiz0rWings);
	}

	/// <summary>
	/// If <see langword="true"/>, then the given item can be worn asymmetrically.
	/// </summary>
	/// <param name="item">The item to check</param>
	/// <returns>Before <see cref="SetStaticDefaults"/> is called, <see langword="false"/>. Otherwise, whether or not <paramref name="item"/> can be worm asymmetrically.</returns>
	public static bool ItemIsAsymmetrical(Item item)
	{
		return ItemIsSpecial(item.type) || (_asymmetrics != null &&
			(_asymmetrics.Any(d => d.id == item.headSlot && d.equipType == EquipType.Head) ||
			_asymmetrics.Any(d => d.id == item.bodySlot && d.equipType == EquipType.Body) ||
			_asymmetrics.Any(d => d.id == item.legSlot && d.equipType == EquipType.Legs) ||
			_asymmetrics.Any(d => d.id == item.handOnSlot && d.equipType == EquipType.HandsOn) ||
			_asymmetrics.Any(d => d.id == item.handOffSlot && d.equipType == EquipType.HandsOff) ||
			_asymmetrics.Any(d => d.id == item.backSlot && d.equipType == EquipType.Back) ||
			_asymmetrics.Any(d => d.id == item.frontSlot && d.equipType == EquipType.Front) ||
			_asymmetrics.Any(d => d.id == item.shoeSlot && d.equipType == EquipType.Shoes) ||
			_asymmetrics.Any(d => d.id == item.waistSlot && d.equipType == EquipType.Waist) ||
			_asymmetrics.Any(d => d.id == item.wingSlot && d.equipType == EquipType.Wings) ||
			_asymmetrics.Any(d => d.id == item.shieldSlot && d.equipType == EquipType.Shield) ||
			_asymmetrics.Any(d => d.id == item.neckSlot && d.equipType == EquipType.Neck) ||
			_asymmetrics.Any(d => d.id == item.faceSlot && d.equipType == EquipType.Face) ||
			_asymmetrics.Any(d => d.id == item.balloonSlot && d.equipType == EquipType.Balloon) ||
			_asymmetrics.Any(d => d.id == item.beardSlot && d.equipType == EquipType.Beard)));
	}

	/// <summary>
	/// Adds a new equip to <see cref="_asymmetrics"/>.
	/// </summary>
	/// <returns><see langword="true"/> if the equip was added successfully, <see langword="false"/> otherwise.</returns>
	/// <param name="data">The data to add.</param>
	internal static bool AddEquip(AsymmetricData data)
	{
		if (_asymmetrics.Any(d => d.Equals(data)))
			return false;

		_asymmetrics.Add(data);
		return true;
	}

	/// <inheritdoc cref="AddEquip(AsymmetricData)"/>
	internal static bool AddEquip(EquipType equipType, int id, int newId = -1, PlayerSide side = PlayerSide.Right)
	{
		return AddEquip(new(equipType, id, newId, side));
	}

	/// <summary>
	/// Registers an item as a special item.
	/// </summary>
	/// <param name="itemId">The item type to register.</param>
	/// <param name="side">The side of the player this item appears on when the player is facing right.</param>
	/// <returns><see langword="false"/> if this item is already registered as special, <see langword="true"/> otherwise.</returns>
	internal static bool AddSpecialItem(int itemId, PlayerSide side = PlayerSide.Right)
	{
		if (ItemIsSpecial(itemId))
		{
			return false;
		}

		_specialItems.Add((itemId, side));
		return true;
	}

	[Pure]
	private static bool ItemIsSpecial(int itemId)
	{
		return _specialItems.Any((pair) => pair.itemId == itemId);
	}

	[Pure]
	internal static PlayerSide SpecialItemDefaultSide(int itemId)
	{
		return ItemIsSpecial(itemId) ? _specialItems.First(p => p.itemId == itemId).side : PlayerSide.Default;
	}
}