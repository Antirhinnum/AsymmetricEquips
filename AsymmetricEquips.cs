using AsymmetricEquips.Common.Systems;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AsymmetricEquips;

public sealed class AsymmetricEquips : Mod
{
	public override object Call(params object[] args)
	{
		if (AsymmetricSystem._finishedEquips)
		{
			Logger.Error("Error: Call must be called before AddRecipes -- PostSetupContent works best.");
			return false;
		}

		Array.Resize(ref args, 5);
		if (args[0] is not string key)
		{
			Logger.Error("Error: The first argument passed into Call must be a string");
			return false;
		}

		try
		{
			switch (key)
			{
				case "AddEquip":
					// Args: EquipType, equip ID, new ID, PlayerSide (0-2)
					AsymmetricSystem.AddEquip((EquipType)Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), Convert.ToInt32(args[3] ?? -1), (PlayerSide)Convert.ToInt32(args[4] ?? PlayerSide.Right));
					break;

				case "AddGlove":
					// Args: item ID
					Item item = ContentSamples.ItemsByType[Convert.ToInt32(args[1])];
					if (item.handOnSlot == -1 || item.handOffSlot == -1)
					{
						Logger.Error("Error: AddGlove was called on item \"" + ItemID.Search.GetName(item.type) + "\", which doesn't set both handOnSlot and handOffSlot");
						return false;
					}
					AsymmetricSystem.AddEquip(EquipType.HandsOn, item.handOnSlot);
					AsymmetricSystem.AddEquip(EquipType.HandsOff, item.handOffSlot, side: PlayerSide.Left);
					break;

				default:
					Logger.Error("Error: Unknown message " + key);
					break;
			}
		}
		catch
		{
			Logger.Error("Error: Call failed for message " + key);
			return false;
		}

		return true;
	}
}