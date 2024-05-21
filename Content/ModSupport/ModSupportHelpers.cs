using AsymmetricEquips.Common.Systems;
using AsymmetricEquips.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AsymmetricEquips.Content.ModSupport;

internal static class ModSupportHelpers
{
	private static Mod Mod => ModLoader.GetMod(nameof(AsymmetricEquips));

	internal static bool ItemTypeSafe(string modName, string itemName, out int type)
	{
		if (ModLoader.TryGetMod(modName, out Mod mod) && mod.TryFind(itemName, out ModItem item))
		{
			type = item.Type;
			return true;
		}

#if DEBUG
		Mod.Logger.DebugFormat("Couldn't get item type {0} from {1}.", itemName, modName);
#endif
		type = 0;
		return false;
	}

	internal static void AddEquip(EquipType type, int id, int newId = -1, PlayerSide side = PlayerSide.Right)
	{
		Mod.Call(nameof(AddEquip), type, id, newId, side);
	}

	internal static void AddGlove(int itemId)
	{
		Mod.Call(nameof(AddGlove), itemId);
	}

	internal static void AddGlove(string modName, string itemName)
	{
		if (ModContent.TryFind(modName, itemName, out ModItem item))
		{
			AddGlove(item.Type);
		}
	}

	internal static (int frontBalloon, int cFrontBalloon, int frontBalloonInner, int cFrontBalloonInner, Vector2 offset) GetFrontBalloon(Player player)
	{
		return ((int, int, int, int, Vector2))Mod.Call(nameof(GetFrontBalloon), player);
	}

	internal static void AddHiddenEquip(string modName, string itemName, EquipType type)
	{
		if (ItemTypeSafe(modName, itemName, out int itemType))
		{
			AddEquip(type, EquipHelper.GetItemEquip(itemType, type));
		}
	}

	internal static void AddHiddenEquip<T>(EquipType type) where T : ModItem
	{
		AddEquip(type, EquipHelper.GetItemEquip<T>(type));
	}

	internal static void AddFlippedEquip(string modName, string itemName, EquipType type, PlayerSide side = PlayerSide.Right)
	{
		if (ItemTypeSafe(modName, itemName, out int itemType))
		{
			int equipId = EquipHelper.GetItemEquip(itemType, type);
			int newId = EquipLoader.GetEquipSlot(Mod, FlippedEquipName(ItemLoader.GetItem(itemType).Name, type), type);
			AddEquip(type, equipId, newId, side);
		}
	}

	internal static void AddFlippedEquip<T>(EquipType type, PlayerSide side = PlayerSide.Right) where T : ModItem
	{
		ModItem item = ModContent.GetInstance<T>();
		AddEquip(type, EquipHelper.GetItemEquip<T>(type), EquipLoader.GetEquipSlot(Mod, FlippedEquipName(item.Name, type), type), side);
	}

	internal static void AddSmallHead(string modName, string itemName, PlayerSide side = PlayerSide.Right)
	{
		if (ItemTypeSafe(modName, itemName, out int itemType))
		{
			AddEquip(EquipType.Head, EquipHelper.GetItemEquip(itemType, EquipType.Head), ArmorIDs.Head.FamiliarWig, side);
		}
	}

	internal static void AddSmallHead<T>(PlayerSide side = PlayerSide.Right) where T : ModItem
	{
		AddEquip(EquipType.Head, EquipHelper.GetItemEquip<T>(EquipType.Head), ArmorIDs.Head.FamiliarWig, side);
	}

	internal static void AddBalloon(int itemId)
	{
		AddEquip(EquipType.Balloon, EquipHelper.GetItemEquip(ContentSamples.ItemsByType[itemId], EquipType.Balloon), side: PlayerSide.Left);
	}

	internal static void AddBalloon(string modName, string itemName)
	{
		if (ItemTypeSafe(modName, itemName, out int itemType))
		{
			AddBalloon(itemType);
		}
	}

	internal static void AddBalloon<T>() where T : ModItem
	{
		AddBalloon(ModContent.ItemType<T>());
	}

	internal static int AddFlippedEquipTexture(string modName, string name, EquipType type)
	{
		return EquipLoader.AddEquipTexture(Mod, FlippedEquipTexturePath(modName, name, type), type, name: FlippedEquipName(name, type));
	}

	private static string FlippedEquipTexturePath(string modName, string name, EquipType type)
	{
		return $"{Mod.Name}/Assets/Textures/{modName}/{FlippedEquipName(name, type)}";
	}

	private static string FlippedEquipName(string name, EquipType type)
	{
		return $"{name}_{type}_Flipped";
	}

	/// <inheritdoc cref="ModSupportBalloonLayer.RegisterBalloonDrawLayer{T}"/>
	public static void RegisterBalloonDrawLayer<T>() where T : PlayerDrawLayer
	{
		ModSupportBalloonLayer.RegisterBalloonDrawLayer<T>();
	}

	public static void RegisterBalloonDrawLayer(string modName, string layerName)
	{
		if (ModContent.TryFind(modName, layerName, out PlayerDrawLayer layer))
		{
			ModSupportBalloonLayer.RegisterBalloonDrawLayer(layer);
		}
	}
}