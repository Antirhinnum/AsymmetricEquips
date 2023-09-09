using AsymmetricEquips.Common.GlobalItems;
using AsymmetricEquips.Common.PlayerLayers;
using AsymmetricEquips.Common.Players;
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
				case "ItemOnFrontSide":
				case "ItemOnDefaultSide":
				{
					// Args: Item, Player
					Item item = (Item)args[1];
					return !item.TryGetGlobalItem(out AsymmetricItem aItem) || aItem.ItemOnDefaultSide(item, (Player)args[2]);
				}
				case "GetFrontBalloon":
				{
					// Args: Player
					Player player = (Player)args[1];
					AsymmetricPlayer aPlayer = player.GetModPlayer<AsymmetricPlayer>();
					return ((int)aPlayer.frontBalloon, aPlayer.cFrontBalloon, (int)aPlayer.frontBalloonInner, aPlayer.cFrontBalloonInner, FrontBalloonPlayerLayer.FrontBalloonOffset(player));
				}
			}
		}
		catch
		{
			Logger.Error("Error: Call failed for message " + key);
			return false;
		}

		if (AsymmetricSystem._finishedEquips)
		{
			Logger.Error($"Error: \"{key}\" is either an unknown key, or one that must be used before AddRecipes. PostSetupContent works best");
			return false;
		}

		try
		{
			switch (key)
			{
				case "AddEquip":
				{
					// Args: EquipType, equip ID, new ID (int?), PlayerSide (int?) (0-2)
					EquipType equipType = (EquipType)Convert.ToInt32(args[1]);
					int id = Convert.ToInt32(args[2]);
					int newId = Convert.ToInt32(args[3] ?? -1);
					PlayerSide side = (PlayerSide)Convert.ToInt32(args[4] ?? PlayerSide.Right);

					if (id < 0)
					{
						Logger.Error($"Error: The passed ID \"{id}\" must be greater than 0");
						return false;
					}

					if (!Enum.IsDefined(side))
					{
						Logger.Error($"Error: The passed side \"{side}\" must be 0, 1, or 2");
						return false;
					}

					AsymmetricSystem.AddEquip(equipType, id, newId, side);
					break;
				}

				case "AddGlove":
				{
					// Args: item ID
					int type = Convert.ToInt32(args[1]);
					if (type <= 0 || type >= ItemLoader.ItemCount)
					{
						Logger.Error($"Error: AddGlove was called with the item ID {type}, which doesn't exist");
						return false;
					}

					Item item = ContentSamples.ItemsByType[type];
					if (item.handOnSlot == -1 || item.handOffSlot == -1)
					{
						Logger.Error("Error: AddGlove was called on item \"" + ItemID.Search.GetName(item.type) + "\", which doesn't set both handOnSlot and handOffSlot");
						return false;
					}
					AsymmetricSystem.AddEquip(EquipType.HandsOn, item.handOnSlot);
					AsymmetricSystem.AddEquip(EquipType.HandsOff, item.handOffSlot, side: PlayerSide.Left);
					break;
				}

				case "AddSpecialItem":
				{
					// Args: item ID
					int type = Convert.ToInt32(args[1]);
					if (type <= 0 || type >= ItemLoader.ItemCount)
					{
						Logger.Error($"Error: AddSpecialItem was called with the item ID {type}, which doesn't exist");
						return false;
					}

					PlayerSide side = (PlayerSide)Convert.ToInt32(args[2] ?? PlayerSide.Right);
					if (!Enum.IsDefined(side))
					{
						Logger.Error($"Error: The passed side \"{side}\" must be 1 or 2");
						return false;
					}

					if (side == PlayerSide.Default)
					{
						Logger.Error($"Error: AddSpecialItem cannot be called with a side of 0.");
						return false;
					}

					AsymmetricSystem.AddSpecialItem(type, side);
					break;
				}

				default:
				{
					Logger.Error("Error: Unknown message " + key);
					break;
				}
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