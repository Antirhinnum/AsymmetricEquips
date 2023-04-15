using AsymmetricEquips.Common.GlobalItems;
using log4net;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AsymmetricEquips.Common.Players;

/// <summary>
/// Handles the specific logic for Yoraiz0r's Spell.
/// </summary>
internal sealed class AsymmetricYoraiz0rEyePlayer : ModPlayer
{
	public override void Load()
	{
		IL_Player.UpdateVisibleAccessory += MakeYoraiz0rEyeAsymmetric;
	}

	public override void Unload()
	{
		IL_Player.UpdateVisibleAccessory -= MakeYoraiz0rEyeAsymmetric;
	}

	private static void MakeYoraiz0rEyeAsymmetric(ILContext il)
	{
		ILCursor c = new(il);
		ILog logger = ModContent.GetInstance<AsymmetricEquips>().Logger;

		// Only set yoraiz0rEye if the eye is on the correct side of the player.
		// Match:
		//	if (item.type == 3580) {
		// Change to:
		// if (item.type == 3580 && (!item.TryGetGlobalItem(out AsymmetricItem aItem) || aItem.FacingCorrectDirection(item, player))) {
		FieldInfo _Item_type = typeof(Item).GetField(nameof(Item.type));
		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(2),
			i => i.MatchLdfld(_Item_type),
			i => i.MatchLdcI4(ItemID.Yoraiz0rWings)
			))
		{
			return;
		}

		c.Emit(OpCodes.Ceq); // This originally goes into a bne.un, so turn it into a bool for our use.
		c.Emit(OpCodes.Ldarg_0); // Load the player and the item.
		c.Emit(OpCodes.Ldarg_2);
		c.EmitDelegate<Func<Player, Item, bool>>((player, item) => !item.TryGetGlobalItem(out AsymmetricItem aItem) || aItem.ItemOnDefaultSide(item, player));
		c.Emit(OpCodes.And); // Original type check && direction check
		c.Emit(OpCodes.Ldc_I4_1); // There's still a bne.un after this, so push a 1 for comparison. If the item is Yoraiz0r's Spell and it's on a visible side, we comparing 1 to 1 and set the field for the eye.
	}
}