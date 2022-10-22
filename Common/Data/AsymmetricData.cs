using AsymmetricEquips.Common.Systems;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader;

namespace AsymmetricEquips.Common.Data;

/// <summary>
/// Used to store info about how an asymmetrical equip should function
/// </summary>
public readonly struct AsymmetricData
{
	/// <summary>
	/// The EquipType of this equip.
	/// </summary>
	public readonly EquipType equipType;

	/// <summary>
	/// The ID of this equip.
	/// </summary>
	public readonly int id;

	/// <summary>
	/// The replacement for this equip's ID.<br/>
	/// Used if an equip should have a different appeared when flipped. Defaults to -1, which removes the equip
	/// </summary>
	public readonly int newId;

	/// <summary>
	/// The default side this equip lies on when the player is facing right
	/// </summary>
	public readonly PlayerSide side;

	public AsymmetricData(EquipType equipType, int id, int newId = -1, PlayerSide side = PlayerSide.Right)
	{
		this.equipType = equipType;
		this.id = id;
		this.newId = newId;
		this.side = side;
	}

	public override bool Equals([NotNullWhen(true)] object obj)
	{
		return obj is AsymmetricData data && Equals(this, data);
	}

	public bool Equals(AsymmetricData other)
	{
		return equipType == other.equipType && id == other.id;
	}

	public override int GetHashCode()
	{
		return ((short)id << 16) & (short)equipType;
	}

	public static bool operator ==(AsymmetricData left, AsymmetricData right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(AsymmetricData left, AsymmetricData right)
	{
		return !(left == right);
	}
}