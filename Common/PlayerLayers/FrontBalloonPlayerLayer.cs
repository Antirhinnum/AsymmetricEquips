using AsymmetricEquips.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AsymmetricEquips.Common.PlayerLayers;

public class FrontBalloonPlayerLayer : PlayerDrawLayer
{
	public override Position GetDefaultPosition()
	{
		return new Multiple()
		{
			{ new Between(PlayerDrawLayers.OffhandAcc, PlayerDrawLayers.ArmOverItem), drawInfo => drawInfo.drawPlayer.TryGetModPlayer(out AsymmetricPlayer aPlayer) && aPlayer.frontBalloon > -1 && !ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer[aPlayer.frontBalloon] },
			{ new Between(PlayerDrawLayers.ArmOverItem, PlayerDrawLayers.HeldItem), drawInfo => drawInfo.drawPlayer.TryGetModPlayer(out AsymmetricPlayer aPlayer) && aPlayer.frontBalloon > -1 && ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer[aPlayer.frontBalloon] }
		};
	}

	public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
	{
		return drawInfo.drawPlayer.TryGetModPlayer(out AsymmetricPlayer aPlayer) && aPlayer.frontBalloon > -1;
	}

	protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		#region Setup

		// Setup drawInfo so that we can use the vanilla balloon drawing method instead of manually drawing.
		int oldBalloon = drawInfo.drawPlayer.balloon;
		int oldCBalloon = drawInfo.cBalloon;
		Vector2 originalPosition = drawInfo.Position; // If missing, then the next layer draw will be offset

		AsymmetricPlayer aPlayer = drawInfo.drawPlayer.GetModPlayer<AsymmetricPlayer>();
		int frontBalloon = aPlayer.frontBalloon;
		int frontBalloonDye = aPlayer.cFrontBalloon;

		if (ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer[frontBalloon])
		{
			frontBalloon = aPlayer.frontBalloonInner;
			frontBalloonDye = aPlayer.cFrontBalloonInner;
		}

		drawInfo.drawPlayer.balloon = frontBalloon;
		drawInfo.cBalloon = frontBalloonDye;

		if (frontBalloon > 0 && frontBalloon < ArmorIDs.Balloon.Count)
		{
			Main.instance.LoadAccBalloon(frontBalloon);
		}

		drawInfo.Position += FrontBalloonOffset(drawInfo.drawPlayer);

		#endregion Setup

		PlayerDrawLayers.DrawPlayer_11_Balloons(ref drawInfo);

		// Reset back to normal
		drawInfo.drawPlayer.balloon = oldBalloon;
		drawInfo.cBalloon = oldCBalloon;
		drawInfo.Position = originalPosition;
	}

	/// <summary>
	/// Gets the offset to apply to <see cref="PlayerDrawSet.Position"/> for a player's front balloon.
	/// </summary>
	/// <param name="player">The player that has the front balloon equipped.</param>
	/// <returns>The offset, accounting for directions and framing.</returns>
	internal static Vector2 FrontBalloonOffset(Player player)
	{
		Vector2 originalOffset = Main.OffsetsPlayerOffhand[player.bodyFrame.Y / 56];
		if (player.direction != 1)
			originalOffset.X = player.width - originalOffset.X;

		if (player.gravDir != 1f)
			originalOffset.Y -= player.height;

		Vector2 newOffset = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56];
		if (player.direction != 1)
			newOffset.X = player.width - newOffset.X;

		if (player.gravDir != 1f)
			newOffset.Y -= player.height;

		// Main.OffsetsPlayerOnhand has odd values in it.
		// These work fine for actual HandOn equips, but they misalign balloons by half a pixel.
		newOffset += new Vector2(newOffset.X % 2f, newOffset.Y % 2f);

		Vector2 finalOffset = Vector2.Zero;
		finalOffset -= originalOffset * new Vector2(1f, player.gravDir);
		finalOffset += newOffset * new Vector2(1f, player.gravDir);

		// An extra offset to help line up the balloon string with the player's hand
		finalOffset.X -= 6f * player.direction;
		return finalOffset;
	}
}