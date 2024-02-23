using System.Numerics;

namespace SimpleModelAnimator.Formats.Extensions;

public static class BinaryWriterExtensions
{
	public static void Write(this BinaryWriter bw, Vector2 vector)
	{
		bw.Write(vector.X);
		bw.Write(vector.Y);
	}

	public static void Write(this BinaryWriter bw, Vector3 vector)
	{
		bw.Write(vector.X);
		bw.Write(vector.Y);
		bw.Write(vector.Z);
	}

	public static void Write(this BinaryWriter bw, Vector4 vector)
	{
		bw.Write(vector.X);
		bw.Write(vector.Y);
		bw.Write(vector.Z);
		bw.Write(vector.W);
	}

	public static void Write(this BinaryWriter bw, Quaternion quaternion)
	{
		bw.Write(quaternion.X);
		bw.Write(quaternion.Y);
		bw.Write(quaternion.Z);
		bw.Write(quaternion.W);
	}

	public static void WriteOptional(this BinaryWriter bw, string? value)
	{
		bw.Write(value != null);
		if (value != null)
			bw.Write(value);
	}
}
