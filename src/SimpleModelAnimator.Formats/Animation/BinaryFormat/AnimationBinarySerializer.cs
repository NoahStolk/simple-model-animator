using SimpleModelAnimator.Formats.Animation.Model;

namespace SimpleModelAnimator.Formats.Animation.BinaryFormat;

public static class AnimationBinarySerializer
{
	private const int _version = 1;

	public static void WriteAnimation(MemoryStream ms, AnimationData animation)
	{
		using BinaryWriter bw = new(ms);

		// Header
		bw.Write(AnimationBinaryConstants.Header);
		bw.Write7BitEncodedInt(_version);
		bw.Write(animation.FramesPerSecond);

		// Sections
		List<Section> sections =
		[
			new(AnimationBinaryConstants.ModelsSectionId, WriteStringListSection(animation.RelativeModelPaths)),
			new(AnimationBinaryConstants.TexturesSectionId, WriteStringListSection(animation.RelativeTexturePaths)),
			new(AnimationBinaryConstants.MeshesSectionId, WriteWorldObjectsSection(animation.Meshes)),
		];

		bw.Write7BitEncodedInt(sections.Count);
		foreach (Section section in sections)
			section.Write(bw);
	}

	private static byte[] WriteStringListSection(IReadOnlyCollection<string> values)
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write7BitEncodedInt(values.Count);
		foreach (string path in values)
			bw.Write(path);

		return ms.ToArray();
	}

	private static byte[] WriteWorldObjectsSection(IReadOnlyCollection<AnimationMesh> animationMeshes)
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write7BitEncodedInt(animationMeshes.Count);
		foreach (AnimationMesh am in animationMeshes)
		{
			bw.Write(am.RelativeModelPath);
			bw.Write(am.MeshName);
			bw.Write(am.TextureName);
		}

		return ms.ToArray();
	}
}
