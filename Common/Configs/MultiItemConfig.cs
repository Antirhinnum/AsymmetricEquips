using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AsymmetricEquips.Common.Configs;

public sealed class MultiItemConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ServerSide;

	[Header($"$Mods.{nameof(AsymmetricEquips)}.Config.Header")]
	[Label($"$Mods.{nameof(AsymmetricEquips)}.Config.{nameof(ShouldAllowMultiItemEquips)}.Label")]
	[Tooltip($"$Mods.{nameof(AsymmetricEquips)}.Config.{nameof(ShouldAllowMultiItemEquips)}.Tooltip")]
	[DefaultValue(false)]
	public bool ShouldAllowMultiItemEquips;
}