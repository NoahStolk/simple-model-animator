namespace SimpleModelAnimator.Formats.Animation.BinaryFormat;

public static class AnimationBinaryConstants
{
	public const int ModelsSectionId = 0;
	public const int TexturesSectionId = 1;
	public const int MeshesSectionId = 2;

	public static ReadOnlySpan<byte> Header => "SMAA"u8;
}
