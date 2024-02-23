using SimpleModelAnimator.Formats.Animation.Model;
using SimpleModelAnimator.Formats.Extensions;

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
		bw.Write7BitEncodedInt(animation.FrameCount);
		bw.Write(animation.FramesPerSecond);
		bw.Write(animation.ObjPath != null);
		if (animation.ObjPath != null)
			bw.Write(animation.ObjPath);

		// Sections
		List<Section> sections =
		[
			new(AnimationBinaryConstants.MeshesSectionId, WriteMeshesSection(animation.Meshes)),
		];

		bw.Write7BitEncodedInt(sections.Count);
		foreach (Section section in sections)
			section.Write(bw);
	}

	private static byte[] WriteMeshesSection(IReadOnlyCollection<AnimationMesh> animationMeshes)
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write7BitEncodedInt(animationMeshes.Count);
		foreach (AnimationMesh am in animationMeshes)
		{
			WriteAnimationMesh(bw, am);
		}

		return ms.ToArray();
	}

	private static void WriteAnimationMesh(BinaryWriter bw, AnimationMesh am)
	{
		bw.Write(am.MeshName);
		bw.Write(am.IsRoot);
		bw.Write(am.Origin);

		bw.Write7BitEncodedInt(am.Children.Count);
		foreach (AnimationMesh child in am.Children)
			WriteAnimationMesh(bw, child);

		bw.Write7BitEncodedInt(am.KeyFrames.Count);
		foreach (AnimationKeyFrame kf in am.KeyFrames)
			WriteAnimationKeyFrame(bw, kf);
	}

	private static void WriteAnimationKeyFrame(BinaryWriter bw, AnimationKeyFrame kf)
	{
		bw.Write7BitEncodedInt(kf.Index);
		bw.Write(kf.Position);
		bw.Write(kf.Rotation);
	}
}
