using AsymmetricEquips.Common.GlobalItems;
using Terraria;

namespace AsymmetricEquips.Common.Data;

/// <summary>
/// A cache of an item's equip slots
/// </summary>
internal readonly struct EquipData
{
	public readonly int head;
	public readonly int body;
	public readonly int legs;
	public readonly sbyte handOn;
	public readonly sbyte handOff;
	public readonly sbyte back;
	public readonly sbyte front;
	public readonly sbyte shoe;
	public readonly sbyte waist;
	public readonly sbyte wing;
	public readonly sbyte shield;
	public readonly sbyte neck;
	public readonly sbyte face;
	public readonly sbyte balloon;
	public readonly sbyte beard;

	// Custom slots
	public readonly sbyte frontBalloon;

	public EquipData(Item item)
	{
		head = item.headSlot;
		body = item.bodySlot;
		legs = item.legSlot;
		handOn = item.handOnSlot;
		handOff = item.handOffSlot;
		back = item.backSlot;
		front = item.frontSlot;
		shoe = item.shoeSlot;
		waist = item.waistSlot;
		wing = item.wingSlot;
		shield = item.shieldSlot;
		neck = item.neckSlot;
		face = item.faceSlot;
		balloon = item.balloonSlot;
		beard = item.beardSlot;

		if (item.TryGetGlobalItem(out AsymmetricItem aItem))
		{
			frontBalloon = aItem.frontBalloonSlot;
		}
		else
		{
			frontBalloon = -1;
		}
	}

	public void Retrieve(Item item)
	{
		item.headSlot = head;
		item.bodySlot = body;
		item.legSlot = legs;
		item.handOnSlot = handOn;
		item.handOffSlot = handOff;
		item.backSlot = back;
		item.frontSlot = front;
		item.shoeSlot = shoe;
		item.waistSlot = waist;
		item.wingSlot = wing;
		item.shieldSlot = shield;
		item.neckSlot = neck;
		item.faceSlot = face;
		item.balloonSlot = balloon;
		item.beardSlot = beard;

		if (item.TryGetGlobalItem(out AsymmetricItem aItem))
		{
			aItem.frontBalloonSlot = frontBalloon;
		}
	}
}