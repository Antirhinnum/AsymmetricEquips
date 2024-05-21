using Terraria;
using Terraria.ModLoader;

namespace AsymmetricEquips.Content.ModSupport.ThoriumMod;

[JITWhenModsEnabled(ModName)]
public sealed class ThoriumModBasicSupport : ModSystem
{
	private const string ModName = "ThoriumMod";

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(ModName);
	}

	public override void OnModLoad()
	{
		if (!Main.dedServ)
		{
			ModSupportHelpers.AddFlippedEquipTexture(ModName, "SupportSash", EquipType.Waist);
			ModSupportHelpers.AddFlippedEquipTexture(ModName, "PiratesPurse", EquipType.Waist);
			ModSupportHelpers.AddFlippedEquipTexture(ModName, "TerrariumWings", EquipType.Wings);
		}
	}

	public override void PostSetupContent()
	{
		ModSupportHelpers.AddHiddenEquip(ModName, "AmberRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "AmethystRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "AquamarineRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "DiamondRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "EmeraldRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "OpalRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "RubyRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "SapphireRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "TopazRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "TheRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "VampireGland", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "HungeringBlossom", EquipType.Face);
		ModSupportHelpers.AddHiddenEquip(ModName, "MetabolicPills", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "PlagueLordFlask", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "ManaBauble", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "CrystalHoney", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "DarkGlaze", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "TheLostCross", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "RingofUnity", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "PotionChaser", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "NecroticSkull", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "SoulStone", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "Canteen", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "DeadEyePatch", EquipType.Face);
		ModSupportHelpers.AddHiddenEquip(ModName, "MermaidCanteen", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "ApothecarysCyanVial", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "ApothecarysScarletVial", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "CrystalArcanite", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "LifeGem", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "NightShadeFlower", EquipType.Face);
		ModSupportHelpers.AddHiddenEquip(ModName, "ThumbRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "MusicPlayerDamage", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "MusicPlayerDamageReduction", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "MusicPlayerLifeRegen", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "MusicPlayerMovementSpeed", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "MusicPlayerNotActivated", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "TunePlayerDamage", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "TunePlayerDamageReduction", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "TunePlayerLifeRegen", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "TunePlayerMovementSpeed", EquipType.Waist);
		ModSupportHelpers.AddHiddenEquip(ModName, "SpikedBracer", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "SpiritBand", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "FragrantCorsage", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "HollowRing", EquipType.HandsOn);
		ModSupportHelpers.AddHiddenEquip(ModName, "BandofReplenishment", EquipType.HandsOn);

		ModSupportHelpers.AddSmallHead(ModName, "NoteHairpin");
		ModSupportHelpers.AddBalloon(ModName, "AutoTuner");
		ModSupportHelpers.AddBalloon(ModName, "DevilsSubwoofer");
		ModSupportHelpers.AddBalloon(ModName, "Subwoofer");
		ModSupportHelpers.AddBalloon(ModName, "TerrariumSurroundSound");
		ModSupportHelpers.AddBalloon(ModName, "HeartOfTheJungle");
		ModSupportHelpers.AddBalloon(ModName, "IncandescentSpark");
		ModSupportHelpers.AddBalloon(ModName, "UpDownBalloon");
		ModSupportHelpers.AddBalloon(ModName, "EyeoftheStorm");
		ModSupportHelpers.AddBalloon(ModName, "MirroroftheBeholder");
		ModSupportHelpers.AddBalloon(ModName, "DarkHeart");
		ModSupportHelpers.AddBalloon(ModName, "Phylactery");
		ModSupportHelpers.AddBalloon(ModName, "OlympicTorch");
		ModSupportHelpers.AddBalloon(ModName, "InnerFlame");

		ModSupportHelpers.AddFlippedEquip(ModName, "SupportSash", EquipType.Waist);
		ModSupportHelpers.AddFlippedEquip(ModName, "PiratesPurse", EquipType.Waist);
		ModSupportHelpers.AddFlippedEquip(ModName, "TerrariumWings", EquipType.Wings);

		ModSupportHelpers.RegisterBalloonDrawLayer(ModName, "BalloonLayer");

		// Sprites will be needed for:
		// - Some HandsOn equips
		// - Some pouch/bag equips
	}
}