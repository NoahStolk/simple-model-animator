using System.Numerics;

namespace SimpleModelAnimator.Formats.Extensions;

public static class BinaryReaderExtensions
{
	public static Vector2 ReadVector2(this BinaryReader br)
	{
		return new(br.ReadSingle(), br.ReadSingle());
	}

	public static Vector3 ReadVector3(this BinaryReader br)
	{
		return new(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
	}

	public static Vector4 ReadVector4(this BinaryReader br)
	{
		return new(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
	}

	public static Quaternion ReadQuaternion(this BinaryReader br)
	{
		return new(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
	}

	public static string? ReadOptionalString(this BinaryReader br)
	{
		return br.ReadBoolean() ? br.ReadString() : null;
	}
}
