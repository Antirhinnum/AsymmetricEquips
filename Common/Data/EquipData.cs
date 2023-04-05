using AsymmetricEquips.Common.GlobalItems;
using Terraria;

namespace AsymmetricEquips.Common.Data;

/// <summary>
/// A cache of an item's equip slots
/// </summary>
internal readonly struct EquipData
{
	public readonly int head = -1;
	public readonly int body = -1;
	public readonly int legs = -1;
	public readonly int handOn = -1;
	public readonly int handOff = -1;
	public readonly int back = -1;
	public readonly int front = -1;
	public readonly int shoe = -1;
	public readonly int waist = -1;
	public readonly int wing = -1;
	public readonly int shield = -1;
	public readonly int neck = -1;
	public readonly int face = -1;
	public readonly int balloon = -1;
	public readonly int beard = -1;

	// Custom slots
	public readonly int frontBalloon;

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