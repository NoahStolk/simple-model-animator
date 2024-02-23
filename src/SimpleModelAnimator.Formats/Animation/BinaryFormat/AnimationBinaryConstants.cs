namespace SimpleModelAnimator.Formats.Animation.BinaryFormat;

public static class AnimationBinaryConstants
{
	public const int MeshesSectionId = 0;

	public static ReadOnlySpan<byte> Header => "SMAA"u8;
}
