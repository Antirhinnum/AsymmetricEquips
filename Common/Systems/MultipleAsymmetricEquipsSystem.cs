using AsymmetricEquips.Common.Configs;
using AsymmetricEquips.Common.GlobalItems;
using log4net;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace AsymmetricEquips.Common.Systems;

/// <summary>
/// By default, vanilla doesn't let you equip more than one of the same item at once, likely for balance reasons.
/// However, by allowing this to happen, players will be able to equip two asymmetric equips and dye them individually. For example, wearing two Nature's Gifts of different colors.
/// This can't realistically be done for normal equips since the player only has one of each armor slot.
/// </summary>
public sealed class MultipleAsymmetricEquipsSystem : ModSystem
{
	public override void Load()
	{
		IL.Terraria.UI.ItemSlot.AccCheck_Inner += AllowMultipleAsymmetricAccessories;
	}

	public override void Unload()
	{
		IL.Terraria.UI.ItemSlot.AccCheck_Inner -= AllowMultipleAsymmetricAccessories;
	}

	private static void AllowMultipleAsymmetricAccessories(ILContext il)
	{
		ILCursor c = new(il);
		ILog logger = ModContent.GetInstance<AsymmetricEquips>().Logger;

		// Get the iteration variable.
		// Match:
		//	for (int i = 0; i < itemCollection.Length; i++) {
		int iterationIndex = -1;
		ILLabel loopBody = null;
		if (!c.TryGotoNext(MoveType.Before,
			i => i.MatchLdloc(out iterationIndex),
			i => i.MatchLdarg(0),
			i => i.MatchLdlen(),
			i => i.MatchConvI4(),
			i => i.MatchBlt(out loopBody)
			))
		{
			// Not worth throwing an exception over.
			logger.Error("Failed multi-item edit #1");
			return;
		}

		// The length check is after the body of the loop, so reset back to the beginning of the body using the label we just found.
		c.GotoLabel(loopBody, MoveType.Before);

		// Only replace the last Item.IsTheSameAs() check, since that's the only one that checks type for other slots.
		// Match:
		//	if (item.IsTheSameAs(itemCollection[i]))
		// Replace with:
		//	if (item.IsTheSameAs(itemCollection[i]) && (items have different side))

		MethodInfo _Item_IsTheSameAs = typeof(Item).GetMethod("IsTheSameAs", BindingFlags.Instance | BindingFlags.NonPublic);
		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(1),
			i => i.MatchLdarg(0),
			i => i.MatchLdloc(iterationIndex), // ldloc.s
			i => i.MatchLdelemRef(),
			i => i.MatchCallvirt(_Item_IsTheSameAs)
			))
		{
			logger.Error("Failed multi-item edit #2");
			return;
		}

		// Load both items
		c.Emit(OpCodes.Ldarg_1);
		c.Emit(OpCodes.Ldarg_0);
		c.Emit(OpCodes.Ldloc_S, (byte)iterationIndex);
		c.Emit(OpCodes.Ldelem_Ref);

		// Compare their Sides.
		// If the two items have different sides, then they should be allowed to be equipped together.
		// Thus, if either item doesn't have a side or if the sides are the same, the code should continue to block them.
		// It should also be blocked if the config item is false.
		c.EmitDelegate<Func<Item, Item, bool>>((item1, item2) =>
		{
			return !ModContent.GetInstance<MultiItemConfig>().ShouldAllowMultiItemEquips
			|| !item1.TryGetGlobalItem(out AsymmetricItem aItem1)
			|| aItem1.Side == PlayerSide.Default
			|| !item2.TryGetGlobalItem(out AsymmetricItem aItem2)
			|| aItem2.Side == PlayerSide.Default
			|| aItem1.Side == aItem2.Side;
		});
		c.Emit(OpCodes.And);
	}
}